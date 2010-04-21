using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using NetGore.Collections;
using NetGore.IO;

namespace NetGore.NPCChat
{
    /// <summary>
    /// Base class for managing the <see cref="NPCChatDialogBase"/>s.
    /// </summary>
    public abstract class NPCChatManagerBase : IEnumerable<NPCChatDialogBase>
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly bool _isReadonly;
        readonly DArray<NPCChatDialogBase> _npcChatDialogs = new DArray<NPCChatDialogBase>(32);

        /// <summary>
        /// Initializes a new instance of the <see cref="NPCChatManagerBase"/> class.
        /// </summary>
        /// <param name="isReadonly">If this manager is read-only.</param>
        protected NPCChatManagerBase(bool isReadonly)
        {
            _isReadonly = isReadonly;
            Load(ContentPaths.Build);
        }

        /// <summary>
        /// Gets the NPCChatDialogBase at the specified index.
        /// </summary>
        /// <param name="id">Index of the NPCChatDialogBase.</param>
        /// <returns>The NPCChatDialogBase at the specified index, or null if invalid.</returns>
        public NPCChatDialogBase this[NPCChatDialogID id]
        {
            get
            {
                // Check for a valid index
                if (!_npcChatDialogs.CanGet((int)id))
                {
                    const string errmsg = "Invalid NPC chat dialog index `{0}`.";
                    if (log.IsErrorEnabled)
                        log.ErrorFormat(errmsg, id);
                    Debug.Fail(string.Format(errmsg, id));
                    return null;
                }

                return _npcChatDialogs[(int)id];
            }
            set
            {
                if (IsReadonly)
                    throw CreateReadonlyException();

                _npcChatDialogs[(int)id] = value;
            }
        }

        /// <summary>
        /// Gets if this manager is read-only.
        /// </summary>
        public bool IsReadonly
        {
            get { return _isReadonly; }
        }

        /// <summary>
        /// When overridden in the derived class, creates a NPCChatDialogBase from the given IValueReader.
        /// </summary>
        /// <param name="reader">IValueReader to read the values from.</param>
        /// <returns>A NPCChatDialogBase created from the given IValueReader.</returns>
        protected abstract NPCChatDialogBase CreateDialog(IValueReader reader);

        /// <summary>
        /// Creates a MethodAccessException to use for when trying to access a method that is cannot be access when read-only. 
        /// </summary>
        /// <returns>A MethodAccessException to use for when trying to access a method that is cannot be
        /// access when read-only.</returns>
        protected static MethodAccessException CreateReadonlyException()
        {
            return new MethodAccessException("Cannot access this method when the NPCChatManagerBase is set to Read-Only.");
        }

        /// <summary>
        /// Gets the path for the data file.
        /// </summary>
        /// <param name="contentPath">ContentPaths to use.</param>
        /// <returns>The path for the data file.</returns>
        protected static string GetFilePath(ContentPaths contentPath)
        {
            return contentPath.Data.Join("npcchat.xml");
        }

        /// <summary>
        /// Loads the data from file.
        /// </summary>
        /// <param name="contentPath">The content path to load the data from.</param>
        void Load(ContentPaths contentPath)
        {
            _npcChatDialogs.Clear();

            var filePath = GetFilePath(contentPath);

            if (!File.Exists(filePath))
            {
                _npcChatDialogs.Trim();
                return;
            }

            var reader = new XmlValueReader(filePath, "ChatDialogs");
            var items = reader.ReadManyNodes("ChatDialogs", CreateDialog);

            for (var i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    _npcChatDialogs[i] = items[i];
            }

            _npcChatDialogs.Trim();
        }

        /// <summary>
        /// Reorganizes the internal buffer to ensure the indices all match up. Only needed if IsReadonly is false
        /// and you don't manually update the indices.
        /// </summary>
        public void Reorganize()
        {
            var dialogs = _npcChatDialogs.ToArray();
            _npcChatDialogs.Clear();

            foreach (var dialog in dialogs)
            {
                _npcChatDialogs[(int)dialog.ID] = dialog;
            }
        }

        /// <summary>
        /// Saves the <see cref="NPCChatDialogBase"/>s in this <see cref="NPCChatManagerBase"/> to file.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        public void Save(ContentPaths contentPath)
        {
            var dialogs = _npcChatDialogs.Where(x => x != null);

            // Write
            var filePath = GetFilePath(contentPath);
            using (var writer = new XmlValueWriter(filePath, "ChatDialogs"))
            {
                writer.WriteManyNodes("ChatDialogs", dialogs, ((w, item) => item.Write(w)));
            }
        }

        #region IEnumerable<NPCChatDialogBase> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<NPCChatDialogBase> GetEnumerator()
        {
            return ((IEnumerable<NPCChatDialogBase>)_npcChatDialogs).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}