using System;
using System.Linq;
using NetGore.Db;
using DemoGame.DbObjs;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Provides a strongly-typed structure for the database table `character_equipped`.
/// </summary>
public class CharacterEquippedTable : ICharacterEquippedTable
{
/// <summary>
/// Array of the database column names.
/// </summary>
 static  readonly System.String[] _dbColumns = new string[] {"character_id", "item_id", "slot" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumns;
}
}
/// <summary>
/// Array of the database column names for columns that are primary keys.
/// </summary>
 static  readonly System.String[] _dbColumnsKeys = new string[] {"character_id", "slot" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns that are primary keys.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbKeyColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumnsKeys;
}
}
/// <summary>
/// Array of the database column names for columns that are not primary keys.
/// </summary>
 static  readonly System.String[] _dbColumnsNonKey = new string[] {"item_id" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns that are not primary keys.
/// </summary>
public static System.Collections.Generic.IEnumerable<System.String> DbNonKeyColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumnsNonKey;
}
}
/// <summary>
/// The name of the database table that this class represents.
/// </summary>
public const System.String TableName = "character_equipped";
/// <summary>
/// The number of columns in the database table that this class represents.
/// </summary>
public const System.Int32 ColumnCount = 3;
/// <summary>
/// The field that maps onto the database column `character_id`.
/// </summary>
System.Int32 _characterID;
/// <summary>
/// The field that maps onto the database column `item_id`.
/// </summary>
System.Int32 _itemID;
/// <summary>
/// The field that maps onto the database column `slot`.
/// </summary>
System.Byte _slot;
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `character_id`.
/// The underlying database type is `int(11)`.
/// </summary>
public DemoGame.Server.CharacterID CharacterID
{
get
{
return (DemoGame.Server.CharacterID)_characterID;
}
set
{
this._characterID = (System.Int32)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `item_id`.
/// The underlying database type is `int(11)`.
/// </summary>
public DemoGame.Server.ItemID ItemID
{
get
{
return (DemoGame.Server.ItemID)_itemID;
}
set
{
this._itemID = (System.Int32)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `slot`.
/// The underlying database type is `tinyint(3) unsigned`.
/// </summary>
public DemoGame.EquipmentSlot Slot
{
get
{
return (DemoGame.EquipmentSlot)_slot;
}
set
{
this._slot = (System.Byte)value;
}
}

/// <summary>
/// Creates a deep copy of this table. All the values will be the same
/// but they will be contained in a different object instance.
/// </summary>
/// <returns>
/// A deep copy of this table.
/// </returns>
public ICharacterEquippedTable DeepCopy()
{
return new CharacterEquippedTable(this);
}
/// <summary>
/// CharacterEquippedTable constructor.
/// </summary>
public CharacterEquippedTable()
{
}
/// <summary>
/// CharacterEquippedTable constructor.
/// </summary>
/// <param name="characterID">The initial value for the corresponding property.</param>
/// <param name="itemID">The initial value for the corresponding property.</param>
/// <param name="slot">The initial value for the corresponding property.</param>
public CharacterEquippedTable(DemoGame.Server.CharacterID @characterID, DemoGame.Server.ItemID @itemID, DemoGame.EquipmentSlot @slot)
{
this.CharacterID = (DemoGame.Server.CharacterID)@characterID;
this.ItemID = (DemoGame.Server.ItemID)@itemID;
this.Slot = (DemoGame.EquipmentSlot)@slot;
}
/// <summary>
/// CharacterEquippedTable constructor.
/// </summary>
/// <param name="source">ICharacterEquippedTable to copy the initial values from.</param>
public CharacterEquippedTable(ICharacterEquippedTable source)
{
CopyValuesFrom(source);
}
/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
/// this method will not create them if they are missing.
/// </summary>
/// <param name="dic">The Dictionary to copy the values into.</param>
public void CopyValues(System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
CopyValues(this, dic);
}
/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
/// this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="dic">The Dictionary to copy the values into.</param>
public static void CopyValues(ICharacterEquippedTable source, System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
dic["@character_id"] = (DemoGame.Server.CharacterID)source.CharacterID;
dic["@item_id"] = (DemoGame.Server.ItemID)source.ItemID;
dic["@slot"] = (DemoGame.EquipmentSlot)source.Slot;
}

/// <summary>
/// Copies the values from the given <paramref name="source"/> into this CharacterEquippedTable.
/// </summary>
/// <param name="source">The ICharacterEquippedTable to copy the values from.</param>
public void CopyValuesFrom(ICharacterEquippedTable source)
{
this.CharacterID = (DemoGame.Server.CharacterID)source.CharacterID;
this.ItemID = (DemoGame.Server.ItemID)source.ItemID;
this.Slot = (DemoGame.EquipmentSlot)source.Slot;
}

/// <summary>
/// Gets the value of a column by the database column's name.
/// </summary>
/// <param name="columnName">The database name of the column to get the value for.</param>
/// <returns>
/// The value of the column with the name <paramref name="columnName"/>.
/// </returns>
public System.Object GetValue(System.String columnName)
{
switch (columnName)
{
case "character_id":
return CharacterID;

case "item_id":
return ItemID;

case "slot":
return Slot;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

/// <summary>
/// Sets the <paramref name="value"/> of a column by the database column's name.
/// </summary>
/// <param name="columnName">The database name of the column to get the <paramref name="value"/> for.</param>
/// <param name="value">Value to assign to the column.</param>
public void SetValue(System.String columnName, System.Object value)
{
switch (columnName)
{
case "character_id":
this.CharacterID = (DemoGame.Server.CharacterID)value;
break;

case "item_id":
this.ItemID = (DemoGame.Server.ItemID)value;
break;

case "slot":
this.Slot = (DemoGame.EquipmentSlot)value;
break;

default:
throw new ArgumentException("Field not found.","columnName");
}
}

/// <summary>
/// Gets the data for the database column that this table represents.
/// </summary>
/// <param name="columnName">The database name of the column to get the data for.</param>
/// <returns>
/// The data for the database column with the name <paramref name="columnName"/>.
/// </returns>
public static ColumnMetadata GetColumnData(System.String columnName)
{
switch (columnName)
{
case "character_id":
return new ColumnMetadata("character_id", "", "int(11)", null, typeof(System.Int32), false, true, false);

case "item_id":
return new ColumnMetadata("item_id", "", "int(11)", null, typeof(System.Int32), false, false, true);

case "slot":
return new ColumnMetadata("slot", "", "tinyint(3) unsigned", null, typeof(System.Byte), false, true, false);

default:
throw new ArgumentException("Field not found.","columnName");
}
}

}

}
