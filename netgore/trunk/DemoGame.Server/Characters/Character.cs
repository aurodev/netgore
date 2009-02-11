using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DemoGame.Extensions;
using log4net;
using Microsoft.Xna.Framework;
using NetGore;

namespace DemoGame.Server
{
    /// <summary>
    /// A game character
    /// </summary>
    public abstract class Character : CharacterEntity, IGetTime, IDynamicEntity
    {
        /// <summary>
        /// Amount of time the character must wait between attacks
        /// </summary>
        const int _attackTimeout = 500; // TODO: Make variable attack rate

        /// <summary>
        /// Random number generator for Characters
        /// </summary>
        static readonly Random _rand = new Random();

        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly World _world;

        /// <summary>
        /// Character's alliance
        /// </summary>
        Alliance _alliance;

        /// <summary>
        /// Body information for the character
        /// </summary>
        BodyInfo _bodyInfo;

        /// <summary>
        /// If the character is alive or not
        /// </summary>
        bool _isAlive = false;

        bool _isLoaded = false;

        /// <summary>
        /// Time at which the character last performed an attack
        /// </summary>
        int _lastAttackTime;

        /// <summary>
        /// Character's state that was last sent to the map clients
        /// </summary>
        CharacterState _lastState;

        /// <summary>
        /// Map the character is currently on
        /// </summary>
        Map _map;

        /// <summary>
        /// Name of the character
        /// </summary>
        string _name;

        /// <summary>
        /// Gets the random number generator for Characters.
        /// </summary>
        protected static Random Rand
        {
            get { return _rand; }
        }

        /// <summary>
        /// Gets or sets (protected) the Character's alliance.
        /// </summary>
        public Alliance Alliance
        {
            get { return _alliance; }
            protected set { _alliance = value; }
        }

        /// <summary>
        /// Gets or sets (protected) the Character's body information.
        /// </summary>
        public BodyInfo BodyInfo
        {
            get { return _bodyInfo; }
            protected set { _bodyInfo = value; }
        }

        /// <summary>
        /// Gets the Character's Inventory.
        /// </summary>
        public abstract Inventory Inventory { get; }

        /// <summary>
        /// Gets or sets (protected) if the Character is currently alive.
        /// </summary>
        public bool IsAlive
        {
            get { return _isAlive; }
            protected set { _isAlive = value; }
        }

        /// <summary>
        /// Gets if the Character has been loaded. If this is false, it is assumed
        /// that changes to stats are because the Character is being loaded, and not that their stats have
        /// changed, and thus will be handled differently.
        /// </summary>
        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        /// <summary>
        /// Gets or sets the map the character is currently on.
        /// </summary>
        public Map Map
        {
            get { return _map; }
            internal set { _map = value; }
        }

        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                Debug.Assert(value != null, "Attempted to give a character a null name.");
                if (Name == value)
                    return;

                // Update the character's name on the clients
                if (Map != null)
                {
                    using (PacketWriter pw = ServerPacket.SetCharName(MapCharIndex, value))
                    {
                        Map.Send(pw);
                    }
                }

                // Store the character's new name
                _name = value;
            }
        }

        /// <summary>
        /// Gets the CharacterStatsBase used for this Character's stats.
        /// </summary>
        public abstract CharacterStatsBase Stats { get; }

        /// <summary>
        /// Gets the world the NPC belongs to.
        /// </summary>
        public World World
        {
            get { return _world; }
        }

        /// <summary>
        /// Character constructor.
        /// </summary>
        /// <param name="world">World that the character belongs to.</param>
        protected Character(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Makes the character perform an attack.
        /// </summary>
        public void Attack()
        {
            int currTime = GetTime();

            // Ensure enough time has elapsed since the last attack
            if (currTime - _lastAttackTime <= _attackTimeout)
                return;

            // Update the last attack time to now
            _lastAttackTime = currTime;

            // Inform the map that the user has performed an attack
            using (PacketWriter charAttack = ServerPacket.CharAttack(MapCharIndex))
            {
                Map.SendToArea(Position, charAttack);
            }

            // Damage all hit characters
            Rectangle hitRect = BodyInfo.GetHitRect(this, BodyInfo.PunchRect);
            var hitChars = Map.GetCharacters(hitRect);
            foreach (Character c in hitChars)
            {
                Attack(c);
            }
        }

        /// <summary>
        /// Tries to attacks a specific target Character. The attack can fail if the target is an invalid
        /// Character for this Character to attack.
        /// </summary>
        /// <param name="target">Character to attack</param>
        void Attack(Character target)
        {
            if (target == null)
            {
                Debug.Fail("target is null.");
                return;
            }

            // Only attack living characters
            if (!target.IsAlive)
                return;

            // Don't attack self
            if (target == this)
                return;

            // Check that the alliance allows the character to attack the target
            if (!Alliance.CanAttack(target.Alliance))
                return;

            // Perform attack
            ushort damage = (ushort)Rand.Next(Stats[StatType.MinHit], Stats[StatType.MaxHit]);
            target.Damage(this, damage);
        }

        /// <summary>
        /// Applies damage to the Character
        /// </summary>
        /// <param name="source">Entity the damage came from (can be null)</param>
        /// <param name="damage">Amount of damage to apply to the Character. Does not include damage reduction
        /// from defense or any other kind of damage alterations since these are calculated here.</param>
        public virtual void Damage(Entity source, int damage)
        {
            // Apply the defence, and ensure the damage is in a valid range
            damage -= Stats[StatType.Defence] / 2;
            if (damage < 1)
                damage = 1;
            else if (damage > Stats[StatType.MaxHP])
                damage = Stats[StatType.MaxHP];

            // Apply damage
            using (PacketWriter pw = ServerPacket.CharDamage(MapCharIndex, damage))
            {
                Map.SendToArea(source.Position, pw);
            }
            Stats[StatType.HP] -= (short)damage;

            // Check if the character died
            if (Stats[StatType.HP] <= 0)
                KilledBy(source);
        }

        /// <summary>
        /// Adds the Character to the World's DisposeStack, allowing for the Dispose to
        /// happen later. It is strongly recommended you use this over Dispose to avoid
        /// InvalidOperation exceptions from the Lists containing the Character.
        /// </summary>
        public void DelayedDispose()
        {
            var stack = Map.World.DisposeStack;
            if (!stack.Contains(this))
                stack.Push(this);
        }

        /// <summary>
        /// Makes the Character drop an existing item.
        /// </summary>
        /// <param name="item">ItemEntity to drop.</param>
        public void DropItem(ItemEntity item)
        {
            Vector2 dropPos = GetDropPos();
            item.Position = dropPos;

            // Add the item to the map
            Map.AddEntity(item);
        }

        /// <summary>
        /// Makes the Character drop an item. Does not modify the item requested to drop at all or anything,
        /// so if you want to also remove the item, such as with dropping an item from the Inventory,
        /// this will not take care of that.
        /// </summary>
        /// <param name="itemTemplate">ItemTemplate for the item to drop.</param>
        /// <param name="amount">Amount of the item to drop.</param>
        protected void DropItem(ItemTemplate itemTemplate, byte amount)
        {
            Vector2 dropPos = GetDropPos();

            // Create the item on the map
            Map.CreateItem(itemTemplate, dropPos, amount);
        }

        Vector2 GetDropPos()
        {
            const int _dropRange = 32;

            // Get the center point of the Character
            Vector2 dropPos = Position + (Size / 2);

            // Move the X point randomly dropRange pixels in either direction
            dropPos.X += -_dropRange + Rand.Next(_dropRange * 2);

            return dropPos;
        }

        /// <summary>
        /// Gets the map interface used by the Character, primarily for when referencing by the CharacterEntity.
        /// Can return null if the Character is not on a map, and null returns should be supported.
        /// </summary>
        /// <returns>Map interface used by the Character. Can be null.</returns>
        protected override IMap GetIMap()
        {
            return Map;
        }

        /// <summary>
        /// Gives an item to the Character.
        /// </summary>
        /// <param name="item">Item to give to the character.</param>
        /// <returns>The remainder of the item that failed to be added to the inventory, or null if all of the
        /// item was added.</returns>
        public abstract ItemEntity GiveItem(ItemEntity item);

        /// <summary>
        /// Checks if a username is valid
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <returns>True if the name is valid, else false</returns>
        public static bool IsValidName(string name)
        {
            return GameData.IsValidCharName(name);
        }

        /// <summary>
        /// Checks if a password is valid
        /// </summary>
        /// <param name="name">Password for the character</param>
        /// <returns>True if the name is valid, else false</returns>
        public static bool IsValidPassword(string name)
        {
            return GameData.IsValidCharName(name);
        }

        /// <summary>
        /// Makes the Character jump (CanJump must be true)
        /// </summary>
        public void Jump()
        {
            if (!CanJump)
                return;

            SetVelocity(Velocity + new Vector2(0.0f, -0.48f));
        }

        /// <summary>
        /// Implements the Character being killed. This doesn't actually care how the Character
        /// was killed, it just takes the appropriate actions to kill them. To handle actions
        /// based on who or what killed the Character, override KilledBy().
        /// </summary>
        public abstract void Kill();

        /// <summary>
        /// Handles the Character dieing by a given source. Used to allow the entity that
        /// killed the NPC to be rewarded, or for the death to be handled differently if needed.
        /// Characters can die without this being called. This is not a substitue for Kill(),
        /// which actually kills off the character. This just provides additional processing
        /// for death by a certain source. KilledBy() will likely always be called after Kill().
        /// </summary>
        /// <param name="source">Entity that killed the Character</param>
        protected virtual void KilledBy(Entity source)
        {
        }

        /// <summary>
        /// Starts moving the character to the left
        /// </summary>
        public void MoveLeft()
        {
            if (IsMovingLeft)
                return;

            SetVelocity(Velocity + new Vector2(-0.18f, 0.0f));
        }

        /// <summary>
        /// Starts moving the character to the right
        /// </summary>
        public void MoveRight()
        {
            if (IsMovingRight)
                return;

            SetVelocity(Velocity + new Vector2(0.18f, 0.0f));
        }

        /// <summary>
        /// Sends the character's velocity and position to all 
        /// the clients on the map
        /// </summary>
        void SendVelocity()
        {
            _lastState = State;

            if (Velocity.X != 0 && Velocity.Y != 0)
            {
                // Velocity X and Y are non-zero
                using (PacketWriter pw = ServerPacket.SetCharVelocity(MapCharIndex, Position, Velocity))
                {
                    Map.Send(pw);
                }
            }
            else if (Velocity.X != 0 && Velocity.Y == 0)
            {
                // Velocity X is non-zero, Y is zero
                using (PacketWriter pw = ServerPacket.SetCharVelocityX(MapCharIndex, Position, Velocity.X))
                {
                    Map.Send(pw);
                }
            }
            else if (Velocity.X == 0 && Velocity.Y != 0)
            {
                // Velocity Y is non-zero, X is zero
                using (PacketWriter pw = ServerPacket.SetCharVelocityY(MapCharIndex, Position, Velocity.Y))
                {
                    Map.Send(pw);
                }
            }
            else
            {
                // Velocity X and Y are zero
                using (PacketWriter pw = ServerPacket.SetCharVelocityZero(MapCharIndex, Position))
                {
                    Map.Send(pw);
                }
            }
        }

        /// <summary>
        /// Sets the Character to being loaded. Must be called after the Character has been loaded.
        /// </summary>
        protected void SetAsLoaded()
        {
            Debug.Assert(!_isLoaded, "SetAsLoaded() has already been called on this Character.");
            _isLoaded = true;
        }

        /// <summary>
        /// Sets the character's heading.
        /// </summary>
        /// <param name="newHeading">New heading for the character.</param>
        public override void SetHeading(Direction newHeading)
        {
            // Confirm the heading has changed
            if (Heading == newHeading)
                return;

            // Change the heading
            base.SetHeading(newHeading);

            // Notify the map
            if (newHeading == Direction.East)
            {
                using (PacketWriter pw = ServerPacket.SetCharHeadingRight(MapCharIndex))
                {
                    Map.Send(pw);
                }
            }
            else
            {
                using (PacketWriter pw = ServerPacket.SetCharHeadingLeft(MapCharIndex))
                {
                    Map.Send(pw);
                }
            }
        }

        /// <summary>
        /// Sets the Character's map.
        /// </summary>
        /// <param name="newMap">Map to place the Character on.</param>
        public void SetMap(Map newMap)
        {
            if (Map == newMap)
            {
                Debug.Fail("Character is already on this map.");
                return;
            }

            if (newMap == null)
                throw new ArgumentNullException("newMap");

            // Remove the Character from the last map
            if (Map != null)
                Map.RemoveEntity(this);

            _map = null;

            // Set the Character's new map
            newMap.AddEntity(this);
        }

        /// <summary>
        /// Stops all of the character's horizontal movement.
        /// </summary>
        public override void StopMoving()
        {
            if (!IsMoving)
                return;

            base.StopMoving();
        }

        /// <summary>
        /// Teleports the character to a new position and informs clients in the area of
        /// interest that the character has teleported.
        /// </summary>
        /// <param name="position">Position to teleport to.</param>
        public override void Teleport(Vector2 position)
        {
            if (Map == null)
            {
                Debug.Fail("Attempted to teleport a character will a null map.");
                return;
            }

            base.Teleport(position);

            // Only send an update message if the character is not dead
            if (IsAlive)
            {
                using (PacketWriter pw = ServerPacket.TeleportChar(MapCharIndex, position))
                {
                    Map.Send(pw);
                }
            }
        }

        /// <summary>
        /// Updates the character and their state.
        /// </summary>
        public override void Update(IMap imap, float deltaTime)
        {
            Debug.Assert(imap == Map, "Character.Update()'s imap is, for whatever reason, not equal to the set Map.");

            if (IsDisposed)
                return;

            base.Update(imap, deltaTime);

            // We must always send an update if the state changed
            if (State != _lastState)
                SendVelocity();
        }

        bool UseEquipment(ItemEntity item, byte? inventorySlot)
        {
            // Only allow users to equip items
            User user = this as User;
            if (user == null)
                return false;

            if (!inventorySlot.HasValue)
            {
                // Equip an item not from the inventory
                return user.Equipped.Equip(item);
            }
            else
            {
                // Equip an item from the inventory
                return user.Equip(inventorySlot.Value);
            }
        }

        /// <summary>
        /// Makes the Character use an item.
        /// </summary>
        /// <param name="item">Item to use.</param>
        /// <param name="inventorySlot">Inventory slot of the item being used, or null if not used from the inventory.</param>
        /// <returns>True if the item was successfully used, else false.</returns>
        public bool UseItem(ItemEntity item, byte? inventorySlot)
        {
            // Check for a valid amount
            if (item.Amount <= 0)
            {
                const string errmsg = "Attempted to use item `{0}`, but the amount was invalid.";
                Debug.Fail(string.Format(errmsg, item));
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, item);
                return false;
            }

            // Use the item based on the item's type
            switch (item.Type)
            {
                case ItemType.Unusable:
                    return false;

                case ItemType.UseOnce:
                    return UseItemUseOnce(item);

                case ItemType.Weapon:
                case ItemType.Helmet:
                case ItemType.Body:
                    return UseEquipment(item, inventorySlot);

                default:
                    // Unhandled item type
                    const string errmsg = "Attempted to use item `{0}`, but it contains invalid or unhandled ItemType `{1}`.";
                    Debug.Fail(string.Format(errmsg, item, item.Type));
                    if (log.IsErrorEnabled)
                        log.ErrorFormat(errmsg, item, item.Type);

                    return false;
            }
        }

        bool UseItemUseOnce(ItemEntity item)
        {
            var useBonuses = item.Stats.Where(stat => stat.Value != 0);
            foreach (IStat stat in useBonuses)
            {
                Stats[stat.StatType] += stat.Value;
            }

            return true;
        }

        #region IDynamicEntity Members

        /// <summary>
        /// Gets the byte array that needs to be sent to the Client to create this Entity on the Client
        /// </summary>
        /// <returns>PacketWriter containing the data used by the Client to create this Entity. Can be null if the Entity
        /// can not be created in it's current state.</returns>
        public abstract PacketWriter GetCreationData();

        /// <summary>
        /// Gets the byte array that needs to be send to the Client to destroy this Entity on the Client.
        /// </summary>
        /// <returns>PacketWriter containing the data used by the Client to destroy this Entity. Can be null if the Entity
        /// can not be destroyed in it's current state.</returns>
        public PacketWriter GetRemovalData()
        {
            // Can't remove what is already dead
            if (!IsAlive)
                return null;

            return ServerPacket.RemoveChar(MapCharIndex);
        }

        #endregion

        #region IGetTime Members

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <returns>Current time.</returns>
        public int GetTime()
        {
            return World.GetTime();
        }

        #endregion
    }
}