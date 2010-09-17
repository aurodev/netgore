using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DemoGame.Server.DbObjs;
using DemoGame.Server.Queries;
using log4net;
using NetGore;
using NetGore.Db;
using NetGore.Network;

namespace DemoGame.Server
{
    /// <summary>
    /// Contains the information for a <see cref="DemoGame.Server.User"/>'s account.
    /// </summary>
    public class UserAccount : AccountTable, IDisposable
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly List<CharacterID> _characterIDs = new List<CharacterID>();
        readonly IDbController _dbController;
        readonly IIPSocket _socket;

        User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="dbController">The <see cref="IDbController"/>.</param>
        UserAccount(IIPSocket socket, IDbController dbController)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (dbController == null)
                throw new ArgumentNullException("dbController");

            _socket = socket;
            _dbController = dbController;
        }

        /// <summary>
        /// Gets the number of Characters in this UserAccount.
        /// </summary>
        public byte CharacterCount
        {
            get { return (byte)_characterIDs.Count; }
        }

        /// <summary>
        /// Gets an IEnumerable of the <see cref="CharacterID"/>s for the <see cref="Character"/>s that are in
        /// this <see cref="UserAccount"/>.
        /// </summary>
        public IEnumerable<CharacterID> CharacterIDs
        {
            get { return _characterIDs; }
        }

        /// <summary>
        /// Gets the IIPSocket that is used to communicate with the client connected to this <see cref="UserAccount"/>.
        /// </summary>
        public IIPSocket Socket
        {
            get { return _socket; }
        }

        /// <summary>
        /// Gets the <see cref="DemoGame.Server.User"/> currently logged in on this <see cref="UserAccount"/>.
        /// </summary>
        public User User
        {
            get { return _user; }
        }

        /// <summary>
        /// Logs out the User from this UserAccount. If the <see cref="User"/> is not null and has not been disposed,
        /// this method will dispose of the User, too.
        /// </summary>
        public void CloseUser()
        {
            ThreadAsserts.IsMainThread();

            var u = _user;
            if (u == null)
                return;

            _user = null;

            if (log.IsInfoEnabled)
                log.InfoFormat("Closed User `{0}` on account `{1}`.", u, this);

            // Make sure the user was disposed
            if (!u.IsDisposed)
                u.Dispose();
        }

        /// <summary>
        /// Encodes a password in a hash.
        /// </summary>
        /// <param name="originalPassword">The original password.</param>
        /// <returns>The hash-encoded password.</returns>
        public static string EncodePassword(string originalPassword)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            var originalBytes = Encoding.Default.GetBytes(originalPassword);
            var encodedBytes = md5.ComputeHash(originalBytes);

            var ret = BitConverter.ToString(encodedBytes);
            ret = ret.Replace("-", string.Empty).ToLower();

            return ret;
        }

        /// <summary>
        /// Gets the CharacterID for a Character in the UserAccount by the given index.
        /// </summary>
        /// <param name="index">The 0-based index of the CharacterID.</param>
        /// <returns>The CharacterID for the Character at the given <paramref name="index"/>.</returns>
        public CharacterID GetCharacterID(byte index)
        {
            return _characterIDs[index];
        }

        /// <summary>
        /// Checks if the given string is a valid string for an account name.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns>True if <paramref name="s"/> is a valid string for an account name; otherwise false.</returns>
        public static bool IsValidName(string s)
        {
            return GameData.AccountName.IsValid(s);
        }

        /// <summary>
        /// Checks if the given string is a valid string for an account password.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns>True if <paramref name="s"/> is a valid string for an account password; otherwise false.</returns>
        public static bool IsValidPassword(string s)
        {
            return GameData.AccountPassword.IsValid(s);
        }

        /// <summary>
        /// Loads the <see cref="CharacterID"/>s for the Characters in this account.
        /// </summary>
        public void LoadCharacterIDs()
        {
            var ids = _dbController.GetQuery<SelectAccountCharacterIDsQuery>().Execute(ID);
            _characterIDs.Clear();
            _characterIDs.AddRange(ids);
        }

        /// <summary>
        /// Tries to log in a UserAccount.
        /// </summary>
        /// <param name="dbController">The DbController.</param>
        /// <param name="socket">The socket used to communicate with the client.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="password">The account password.</param>
        /// <param name="userAccount">When this method returns <see cref="AccountLoginResult.Successful"/>,
        /// contains the <see cref="UserAccount"/> that was logged in to. Otherwise, this value will be null.</param>
        /// <returns>If <see cref="AccountLoginResult.Successful"/>, the login was successful. Otherwise, contains
        /// the reason why the login failed.</returns>
        public static AccountLoginResult Login(IDbController dbController, IIPSocket socket, string name, string password,
                                               out UserAccount userAccount)
        {
            userAccount = new UserAccount(socket, dbController);

            // Try to load the account data
            if (!dbController.GetQuery<SelectAccountQuery>().TryExecute(name, password, userAccount))
            {
                userAccount = null;
                return AccountLoginResult.InvalidName;
            }

            // Check for a matching password
            if (!EncodePassword(password).Equals(userAccount.Password, StringComparison.OrdinalIgnoreCase))
            {
                userAccount = null;
                return AccountLoginResult.InvalidPassword;
            }

            // Check for in use already
            if (userAccount.CurrentIp.HasValue)
            {
                userAccount = null;
                return AccountLoginResult.AccountInUse;
            }

            // Try to mark the account as in use
            if (!dbController.GetQuery<TrySetAccountIPIfNullQuery>().Execute(userAccount.ID, socket.IP))
            {
                userAccount = null;
                return AccountLoginResult.AccountInUse;
            }

            // Get the characters in this account
            userAccount.LoadCharacterIDs();

            dbController.GetQuery<InsertAccountIPQuery>().Execute(userAccount.ID, socket.IP);

            return AccountLoginResult.Successful;
        }

        /// <summary>
        /// Sends the <see cref="AccountCharacterInfo"/>s for the <see cref="Character"/>s in this account to the
        /// client.
        /// </summary>
        public void SendAccountCharacterInfos()
        {
            var charInfos = new AccountCharacterInfo[CharacterCount];
            for (var i = 0; i < charInfos.Length; i++)
            {
                var characterID = _characterIDs[i];
                charInfos[i] = _dbController.GetQuery<SelectAccountCharacterInfoQuery>().Execute(characterID, (byte)i);
            }

            using (var pw = ServerPacket.SendAccountCharacters(charInfos))
            {
                Socket.Send(pw, ServerMessageType.System);
            }
        }

        /// <summary>
        /// Loads and sets the User being used by this account.
        /// </summary>
        /// <param name="world">The World that the User will be part of.</param>
        /// <param name="characterID">The CharacterID of the user to use.</param>
        public void SetUser(World world, CharacterID characterID)
        {
            ThreadAsserts.IsMainThread();

            // Make sure the user is not already set
            if (User != null)
            {
                const string errmsg = "Cannot use SetUser when the User is not null.";
                Debug.Fail(errmsg);
                if (log.IsErrorEnabled)
                    log.Error(errmsg);
                return;
            }

            // Make sure the CharacterID is an ID of a character belonging to this UserAccount
            if (!_characterIDs.Contains(characterID))
            {
                const string errmsg = "Cannot use CharacterID `{0}` - that character does not belong to this UserAccount.";
                Debug.Fail(string.Format(errmsg, characterID));
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, characterID);
                return;
            }

            // Load the User
            _user = new User(_socket, world, characterID);

            if (log.IsInfoEnabled)
                log.InfoFormat("Set User `{0}` on account `{1}`.", _user, this);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, ID);
        }

        /// <summary>
        /// Tries to add a <see cref="Character"/> to a <see cref="UserAccount"/>.
        /// </summary>
        /// <param name="dbController">The <see cref="IDbController"/>.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="errorMsg">If this method returns false, contains the error message.</param>
        /// <returns>True if the character was successfully added to the account; otherwise false.</returns>
        public static bool TryAddCharacter(IDbController dbController, string accountName, string characterName,
                                           out string errorMsg)
        {
            var idCreator = dbController.GetQuery<CharacterIDCreator>();
            var characterID = idCreator.GetNext();

            var success = TryAddCharacter(dbController, accountName, characterName, characterID, out errorMsg);

            if (!success)
                idCreator.FreeID(characterID);

            return success;
        }

        /// <summary>
        /// Tries to add a <see cref="Character"/> to a <see cref="UserAccount"/>.
        /// </summary>
        /// <param name="dbController">The <see cref="IDbController"/>.</param>
        /// <param name="accountName">The account name.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="characterID">The character ID.</param>
        /// <param name="errorMsg">If this method returns false, contains the error message.</param>
        /// <returns>True if the character was successfully added to the account; otherwise false.</returns>
        static bool TryAddCharacter(IDbController dbController, string accountName, string characterName, CharacterID characterID,
                                    out string errorMsg)
        {
            // Try to execute the query
            if (
                !dbController.GetQuery<CreateUserOnAccountQuery>().TryExecute(accountName, characterID, characterName,
                                                                              out errorMsg))
                return false;

            errorMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// Tries to create a new user account.
        /// </summary>
        /// <param name="dbController">The DbController</param>
        /// <param name="socket">The socket containing the connection trying to create the account. Can be null.</param>
        /// <param name="name">The account name.</param>
        /// <param name="password">The account password.</param>
        /// <param name="email">The account email address.</param>
        /// <param name="accountID">When this method returns true, contains the AccountID for the created
        /// <see cref="UserAccount"/>.</param>
        /// <param name="failReason">When this method returns false, contains a <see cref="GameMessage"/> that will be
        /// used to describe why the account failed to be created.</param>
        /// <returns>True if the account was successfully created; otherwise false.</returns>
        public static bool TryCreateAccount(IDbController dbController, IIPSocket socket, string name, string password,
                                            string email, out AccountID accountID, out GameMessage failReason)
        {
            accountID = new AccountID(0);

            // Check for valid values
            if (!GameData.AccountName.IsValid(name))
            {
                failReason = GameMessage.CreateAccountInvalidName;
                return false;
            }

            if (!GameData.AccountPassword.IsValid(password))
            {
                failReason = GameMessage.CreateAccountInvalidPassword;
                return false;
            }

            if (!GameData.AccountEmail.IsValid(email))
            {
                failReason = GameMessage.CreateAccountInvalidEmail;
                return false;
            }

            // Get the IP to use
            var ip = socket != null ? socket.IP : GameData.DefaultCreateAccountIP;

            // Check if too many accounts have been created from this IP
            if (ip != GameData.DefaultCreateAccountIP)
            {
                var recentCreatedCount = dbController.GetQuery<CountRecentlyCreatedAccounts>().Execute(ip);
                if (recentCreatedCount >= ServerConfig.MaxRecentlyCreatedAccounts)
                {
                    failReason = GameMessage.CreateAccountTooManyCreated;
                    return false;
                }
            }

            // Check if the account exists
            var existingAccountID = dbController.GetQuery<SelectAccountIDFromNameQuery>().Execute(name);
            if (existingAccountID.HasValue)
            {
                failReason = GameMessage.CreateAccountAlreadyExists;
                return false;
            }

            // Get the account ID
            var idCreator = dbController.GetQuery<AccountIDCreator>();
            accountID = idCreator.GetNext();

            // Try to execute the query
            var success = dbController.GetQuery<CreateAccountQuery>().TryExecute(accountID, name, password, email, ip);

            // If unsuccessful, free the account ID so it can be reused
            if (!success)
                idCreator.FreeID(accountID);

            failReason = GameMessage.CreateAccountUnknownError;
            return success;
        }

        /// <summary>
        /// Tries to get the AccountID for the account with the given name.
        /// </summary>
        /// <param name="dbController">The DbController.</param>
        /// <param name="accountName">The name of the account.</param>
        /// <param name="accountID">When the method returns true, contains the ID of the account with the given
        /// <paramref name="accountName"/>.</param>
        /// <returns>True if the <paramref name="accountID"/> was found; otherwise false.</returns>
        public static bool TryGetAccountID(IDbController dbController, string accountName, out AccountID accountID)
        {
            var value = dbController.GetQuery<SelectAccountIDFromNameQuery>().Execute(accountName);

            if (!value.HasValue)
            {
                accountID = new AccountID(0);
                return false;
            }
            else
            {
                accountID = value.Value;
                return true;
            }
        }

        /// <summary>
        /// Tries to get the CharacterID for a Character in the UserAccount by the given index.
        /// </summary>
        /// <param name="index">The 0-based index of the CharacterID.</param>
        /// <param name="value">The CharacterID for the Character at the given <paramref name="index"/>.</param>
        /// <returns>True if the <paramref name="value"/> was successfully acquired; otherwise false.</returns>
        public bool TryGetCharacterID(byte index, out CharacterID value)
        {
            if (index < 0 || index >= _characterIDs.Count)
            {
                value = new CharacterID(0);
                return false;
            }

            value = _characterIDs[index];
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disconnectMessage">The message to use for when disconnecting the socket. When disposing an active connection,
        /// this can provide the client a reason why they were disconnected. The default is
        /// <see cref="GameMessage.DisconnectUserDisposed"/>.</param>
        /// <param name="p">The arguments for the <paramref name="disconnectMessage"/>.</param>
        public void Dispose(GameMessage disconnectMessage, params object[] p)
        {
            ThreadAsserts.IsMainThread();

            // Make sure the User is closed
            CloseUser();

            // Break the connection, if connected
            if (Socket != null)
                Socket.Disconnect(disconnectMessage, p);

            // Log the account out in the database
            _dbController.GetQuery<SetAccountCurrentIPNullQuery>().Execute(ID);

            if (log.IsInfoEnabled)
                log.InfoFormat("Disposed account `{0}`.", this);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(GameMessage.DisconnectUserDisposed);
        }

        #endregion
    }
}