/********************************************************************
                   DO NOT MANUALLY EDIT THIS FILE!

This file was automatically generated using the DbClassCreator
program. The only time you should ever alter this file is if you are
using an automated code formatter. The DbClassCreator will overwrite
this file every time it is run, so all manual changes will be lost.
If there is something in this file that you wish to change, you should
be able to do it through the DbClassCreator arguments.

Make sure that you re-run the DbClassCreator every time you alter your
game's database.

For more information on the DbClassCreator, please see:
    http://www.netgore.com/wiki/dbclasscreator.html

This file was generated on (UTC): 3/30/2010 12:13:03 AM
********************************************************************/

using System;
using System.Data;
using System.Linq;
using DemoGame.DbObjs;
using NetGore;
using NetGore.AI;
using NetGore.Db;
using NetGore.Features.Shops;
using NetGore.NPCChat;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Contains extension methods for class NpcCharacterTable that assist in performing
    /// reads and writes to and from a database.
    /// </summary>
    public static class NpcCharacterTableDbExtensions
    {
        /// <summary>
        /// Copies the column values into the given DbParameterValues using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
        ///  this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="paramValues">The DbParameterValues to copy the values into.</param>
        public static void CopyValues(this INpcCharacterTable source, DbParameterValues paramValues)
        {
            paramValues["@account_id"] = (int?)source.AccountID;
            paramValues["@ai_id"] = (ushort?)source.AIID;
            paramValues["@body_id"] = (UInt16)source.BodyID;
            paramValues["@cash"] = source.Cash;
            paramValues["@character_template_id"] = (ushort?)source.CharacterTemplateID;
            paramValues["@chat_dialog"] = (ushort?)source.ChatDialog;
            paramValues["@exp"] = source.Exp;
            paramValues["@hp"] = (Int16)source.HP;
            paramValues["@id"] = source.ID;
            paramValues["@level"] = source.Level;
            paramValues["@map_id"] = (UInt16)source.MapID;
            paramValues["@mp"] = (Int16)source.MP;
            paramValues["@name"] = source.Name;
            paramValues["@respawn_map"] = (ushort?)source.RespawnMap;
            paramValues["@respawn_x"] = source.RespawnX;
            paramValues["@respawn_y"] = source.RespawnY;
            paramValues["@shop_id"] = (ushort?)source.ShopID;
            paramValues["@statpoints"] = source.StatPoints;
            paramValues["@stat_agi"] = source.StatAgi;
            paramValues["@stat_defence"] = source.StatDefence;
            paramValues["@stat_int"] = source.StatInt;
            paramValues["@stat_maxhit"] = source.StatMaxhit;
            paramValues["@stat_maxhp"] = source.StatMaxhp;
            paramValues["@stat_maxmp"] = source.StatMaxmp;
            paramValues["@stat_minhit"] = source.StatMinhit;
            paramValues["@stat_str"] = source.StatStr;
            paramValues["@x"] = source.X;
            paramValues["@y"] = source.Y;
        }

        /// <summary>
        /// Checks if this <see cref="INpcCharacterTable"/> contains the same values as an<paramref name="other"/> <see cref="INpcCharacterTable"/>.
        /// </summary>
        /// <param name="other">The <see cref="INpcCharacterTable"/> to compare the values to.</param>
        /// <returns>
        /// True if this <see cref="INpcCharacterTable"/> contains the same values as the <paramref name="<paramref name="other"/>"/>; <paramref name="other"/>wise false.
        /// </returns>
        public static Boolean HasSameValues(this INpcCharacterTable source, INpcCharacterTable other)
        {
            return Equals(source.AccountID, other.AccountID) && Equals(source.AIID, other.AIID) &&
                   Equals(source.BodyID, other.BodyID) && Equals(source.Cash, other.Cash) &&
                   Equals(source.CharacterTemplateID, other.CharacterTemplateID) && Equals(source.ChatDialog, other.ChatDialog) &&
                   Equals(source.Exp, other.Exp) && Equals(source.HP, other.HP) && Equals(source.ID, other.ID) &&
                   Equals(source.Level, other.Level) && Equals(source.MapID, other.MapID) && Equals(source.MP, other.MP) &&
                   Equals(source.Name, other.Name) && Equals(source.RespawnMap, other.RespawnMap) &&
                   Equals(source.RespawnX, other.RespawnX) && Equals(source.RespawnY, other.RespawnY) &&
                   Equals(source.ShopID, other.ShopID) && Equals(source.StatPoints, other.StatPoints) &&
                   Equals(source.StatAgi, other.StatAgi) && Equals(source.StatDefence, other.StatDefence) &&
                   Equals(source.StatInt, other.StatInt) && Equals(source.StatMaxhit, other.StatMaxhit) &&
                   Equals(source.StatMaxhp, other.StatMaxhp) && Equals(source.StatMaxmp, other.StatMaxmp) &&
                   Equals(source.StatMinhit, other.StatMinhit) && Equals(source.StatStr, other.StatStr) &&
                   Equals(source.X, other.X) && Equals(source.Y, other.Y);
        }

        /// <summary>
        /// Reads the values from an IDataReader and assigns the read values to this
        /// object's properties. The database column's name is used to as the key, so the value
        /// will not be found if any aliases are used or not all columns were selected.
        /// </summary>
        /// <param name="source">The object to add the extension method to.</param>
        /// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
        public static void ReadValues(this NpcCharacterTable source, IDataReader dataReader)
        {
            Int32 i;

            i = dataReader.GetOrdinal("account_id");

            source.AccountID = (AccountID?)(dataReader.IsDBNull(i) ? (int?)null : dataReader.GetInt32(i));

            i = dataReader.GetOrdinal("ai_id");

            source.AIID = (AIID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));

            i = dataReader.GetOrdinal("body_id");

            source.BodyID = (BodyID)dataReader.GetUInt16(i);

            i = dataReader.GetOrdinal("cash");

            source.Cash = dataReader.GetInt32(i);

            i = dataReader.GetOrdinal("character_template_id");

            source.CharacterTemplateID = (CharacterTemplateID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));

            i = dataReader.GetOrdinal("chat_dialog");

            source.ChatDialog = (NPCChatDialogID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));

            i = dataReader.GetOrdinal("exp");

            source.Exp = dataReader.GetInt32(i);

            i = dataReader.GetOrdinal("hp");

            source.HP = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("id");

            source.ID = dataReader.GetInt32(i);

            i = dataReader.GetOrdinal("level");

            source.Level = dataReader.GetByte(i);

            i = dataReader.GetOrdinal("map_id");

            source.MapID = (MapID)dataReader.GetUInt16(i);

            i = dataReader.GetOrdinal("mp");

            source.MP = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("name");

            source.Name = dataReader.GetString(i);

            i = dataReader.GetOrdinal("respawn_map");

            source.RespawnMap = (MapID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));

            i = dataReader.GetOrdinal("respawn_x");

            source.RespawnX = dataReader.GetFloat(i);

            i = dataReader.GetOrdinal("respawn_y");

            source.RespawnY = dataReader.GetFloat(i);

            i = dataReader.GetOrdinal("shop_id");

            source.ShopID = (ShopID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));

            i = dataReader.GetOrdinal("statpoints");

            source.StatPoints = dataReader.GetInt32(i);

            i = dataReader.GetOrdinal("stat_agi");

            source.StatAgi = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_defence");

            source.StatDefence = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_int");

            source.StatInt = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_maxhit");

            source.StatMaxhit = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_maxhp");

            source.StatMaxhp = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_maxmp");

            source.StatMaxmp = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_minhit");

            source.StatMinhit = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("stat_str");

            source.StatStr = dataReader.GetInt16(i);

            i = dataReader.GetOrdinal("x");

            source.X = dataReader.GetFloat(i);

            i = dataReader.GetOrdinal("y");

            source.Y = dataReader.GetFloat(i);
        }

        /// <summary>
        /// Copies the column values into the given DbParameterValues using the database column name
        /// with a prefixed @ as the key. The key must already exist in the DbParameterValues
        /// for the value to be copied over. If any of the keys in the DbParameterValues do not
        /// match one of the column names, or if there is no field for a key, then it will be
        /// ignored. Because of this, it is important to be careful when using this method
        /// since columns or keys can be skipped without any indication.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="paramValues">The DbParameterValues to copy the values into.</param>
        public static void TryCopyValues(this INpcCharacterTable source, DbParameterValues paramValues)
        {
            for (int i = 0; i < paramValues.Count; i++)
            {
                switch (paramValues.GetParameterName(i))
                {
                    case "@account_id":
                        paramValues[i] = (int?)source.AccountID;
                        break;

                    case "@ai_id":
                        paramValues[i] = (ushort?)source.AIID;
                        break;

                    case "@body_id":
                        paramValues[i] = (UInt16)source.BodyID;
                        break;

                    case "@cash":
                        paramValues[i] = source.Cash;
                        break;

                    case "@character_template_id":
                        paramValues[i] = (ushort?)source.CharacterTemplateID;
                        break;

                    case "@chat_dialog":
                        paramValues[i] = (ushort?)source.ChatDialog;
                        break;

                    case "@exp":
                        paramValues[i] = source.Exp;
                        break;

                    case "@hp":
                        paramValues[i] = (Int16)source.HP;
                        break;

                    case "@id":
                        paramValues[i] = source.ID;
                        break;

                    case "@level":
                        paramValues[i] = source.Level;
                        break;

                    case "@map_id":
                        paramValues[i] = (UInt16)source.MapID;
                        break;

                    case "@mp":
                        paramValues[i] = (Int16)source.MP;
                        break;

                    case "@name":
                        paramValues[i] = source.Name;
                        break;

                    case "@respawn_map":
                        paramValues[i] = (ushort?)source.RespawnMap;
                        break;

                    case "@respawn_x":
                        paramValues[i] = source.RespawnX;
                        break;

                    case "@respawn_y":
                        paramValues[i] = source.RespawnY;
                        break;

                    case "@shop_id":
                        paramValues[i] = (ushort?)source.ShopID;
                        break;

                    case "@statpoints":
                        paramValues[i] = source.StatPoints;
                        break;

                    case "@stat_agi":
                        paramValues[i] = source.StatAgi;
                        break;

                    case "@stat_defence":
                        paramValues[i] = source.StatDefence;
                        break;

                    case "@stat_int":
                        paramValues[i] = source.StatInt;
                        break;

                    case "@stat_maxhit":
                        paramValues[i] = source.StatMaxhit;
                        break;

                    case "@stat_maxhp":
                        paramValues[i] = source.StatMaxhp;
                        break;

                    case "@stat_maxmp":
                        paramValues[i] = source.StatMaxmp;
                        break;

                    case "@stat_minhit":
                        paramValues[i] = source.StatMinhit;
                        break;

                    case "@stat_str":
                        paramValues[i] = source.StatStr;
                        break;

                    case "@x":
                        paramValues[i] = source.X;
                        break;

                    case "@y":
                        paramValues[i] = source.Y;
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the values from an IDataReader and assigns the read values to this
        /// object's properties. Unlike ReadValues(), this method not only doesn't require
        /// all values to be in the IDataReader, but also does not require the values in
        /// the IDataReader to be a defined field for the table this class represents.
        /// Because of this, you need to be careful when using this method because values
        /// can easily be skipped without any indication.
        /// </summary>
        /// <param name="source">The object to add the extension method to.</param>
        /// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
        public static void TryReadValues(this NpcCharacterTable source, IDataReader dataReader)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                switch (dataReader.GetName(i))
                {
                    case "account_id":
                        source.AccountID = (AccountID?)(dataReader.IsDBNull(i) ? (int?)null : dataReader.GetInt32(i));
                        break;

                    case "ai_id":
                        source.AIID = (AIID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));
                        break;

                    case "body_id":
                        source.BodyID = (BodyID)dataReader.GetUInt16(i);
                        break;

                    case "cash":
                        source.Cash = dataReader.GetInt32(i);
                        break;

                    case "character_template_id":
                        source.CharacterTemplateID =
                            (CharacterTemplateID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));
                        break;

                    case "chat_dialog":
                        source.ChatDialog = (NPCChatDialogID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));
                        break;

                    case "exp":
                        source.Exp = dataReader.GetInt32(i);
                        break;

                    case "hp":
                        source.HP = dataReader.GetInt16(i);
                        break;

                    case "id":
                        source.ID = dataReader.GetInt32(i);
                        break;

                    case "level":
                        source.Level = dataReader.GetByte(i);
                        break;

                    case "map_id":
                        source.MapID = (MapID)dataReader.GetUInt16(i);
                        break;

                    case "mp":
                        source.MP = dataReader.GetInt16(i);
                        break;

                    case "name":
                        source.Name = dataReader.GetString(i);
                        break;

                    case "respawn_map":
                        source.RespawnMap = (MapID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));
                        break;

                    case "respawn_x":
                        source.RespawnX = dataReader.GetFloat(i);
                        break;

                    case "respawn_y":
                        source.RespawnY = dataReader.GetFloat(i);
                        break;

                    case "shop_id":
                        source.ShopID = (ShopID?)(dataReader.IsDBNull(i) ? (ushort?)null : dataReader.GetUInt16(i));
                        break;

                    case "statpoints":
                        source.StatPoints = dataReader.GetInt32(i);
                        break;

                    case "stat_agi":
                        source.StatAgi = dataReader.GetInt16(i);
                        break;

                    case "stat_defence":
                        source.StatDefence = dataReader.GetInt16(i);
                        break;

                    case "stat_int":
                        source.StatInt = dataReader.GetInt16(i);
                        break;

                    case "stat_maxhit":
                        source.StatMaxhit = dataReader.GetInt16(i);
                        break;

                    case "stat_maxhp":
                        source.StatMaxhp = dataReader.GetInt16(i);
                        break;

                    case "stat_maxmp":
                        source.StatMaxmp = dataReader.GetInt16(i);
                        break;

                    case "stat_minhit":
                        source.StatMinhit = dataReader.GetInt16(i);
                        break;

                    case "stat_str":
                        source.StatStr = dataReader.GetInt16(i);
                        break;

                    case "x":
                        source.X = dataReader.GetFloat(i);
                        break;

                    case "y":
                        source.Y = dataReader.GetFloat(i);
                        break;
                }
            }
        }
    }
}