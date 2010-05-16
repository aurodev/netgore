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

This file was generated on (UTC): 5/16/2010 7:45:19 PM
********************************************************************/

using System;
using System.Linq;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Interface for a class that can be used to serialize values to the database table `game_constant`.
    /// </summary>
    public interface IGameConstantTable
    {
        /// <summary>
        /// Gets the value of the database column `max_account_name_length`.
        /// </summary>
        Byte MaxAccountNameLength { get; }

        /// <summary>
        /// Gets the value of the database column `max_account_password_length`.
        /// </summary>
        Byte MaxAccountPasswordLength { get; }

        /// <summary>
        /// Gets the value of the database column `max_character_name_length`.
        /// </summary>
        Byte MaxCharacterNameLength { get; }

        /// <summary>
        /// Gets the value of the database column `max_characters_per_account`.
        /// </summary>
        Byte MaxCharactersPerAccount { get; }

        /// <summary>
        /// Gets the value of the database column `max_inventory_size`.
        /// </summary>
        Byte MaxInventorySize { get; }

        /// <summary>
        /// Gets the value of the database column `max_shop_items`.
        /// </summary>
        Byte MaxShopItems { get; }

        /// <summary>
        /// Gets the value of the database column `max_status_effect_power`.
        /// </summary>
        UInt16 MaxStatusEffectPower { get; }

        /// <summary>
        /// Gets the value of the database column `min_account_name_length`.
        /// </summary>
        Byte MinAccountNameLength { get; }

        /// <summary>
        /// Gets the value of the database column `min_account_password_length`.
        /// </summary>
        Byte MinAccountPasswordLength { get; }

        /// <summary>
        /// Gets the value of the database column `min_character_name_length`.
        /// </summary>
        Byte MinCharacterNameLength { get; }

        /// <summary>
        /// Gets the value of the database column `screen_height`.
        /// </summary>
        UInt16 ScreenHeight { get; }

        /// <summary>
        /// Gets the value of the database column `screen_width`.
        /// </summary>
        UInt16 ScreenWidth { get; }

        /// <summary>
        /// Gets the value of the database column `server_ip`.
        /// </summary>
        String ServerIp { get; }

        /// <summary>
        /// Gets the value of the database column `server_ping_port`.
        /// </summary>
        UInt16 ServerPingPort { get; }

        /// <summary>
        /// Gets the value of the database column `server_tcp_port`.
        /// </summary>
        UInt16 ServerTcpPort { get; }

        /// <summary>
        /// Gets the value of the database column `world_physics_update_rate`.
        /// </summary>
        UInt16 WorldPhysicsUpdateRate { get; }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        IGameConstantTable DeepCopy();
    }
}