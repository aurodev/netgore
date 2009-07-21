using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NetGore;

namespace DemoGame.Server
{
    public class ItemTemplate
    {
        readonly IEnumerable<StatTypeValue> _baseStats;
        readonly string _desc;
        readonly GrhIndex _graphic;
        readonly byte _height;
        readonly SPValueType _hp;
        readonly ItemTemplateID _id;
        readonly SPValueType _mp;
        readonly string _name;
        readonly IEnumerable<StatTypeValue> _reqStats;
        readonly ItemType _type;
        readonly int _value;
        readonly byte _width;

        public IEnumerable<StatTypeValue> BaseStats
        {
            get { return _baseStats; }
        }

        public string Description
        {
            get { return _desc; }
        }

        public GrhIndex Graphic
        {
            get { return _graphic; }
        }

        public byte Height
        {
            get { return _height; }
        }

        public SPValueType HP
        {
            get { return _hp; }
        }

        public ItemTemplateID ID
        {
            get { return _id; }
        }

        public SPValueType MP
        {
            get { return _mp; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<StatTypeValue> ReqStats
        {
            get { return _reqStats; }
        }

        public Vector2 Size
        {
            get { return new Vector2(_width, _height); }
        }

        public ItemType Type
        {
            get { return _type; }
        }

        public int Value
        {
            get { return _value; }
        }

        public byte Width
        {
            get { return _width; }
        }

        public ItemTemplate(ItemTemplateID id, string name, string desc, ItemType type, GrhIndex graphic, int value, byte width,
                            byte height, SPValueType hp, SPValueType mp, IEnumerable<StatTypeValue> baseStats,
                            IEnumerable<StatTypeValue> reqStats)
        {
            _id = id;
            _name = name;
            _desc = desc;
            _type = type;
            _graphic = graphic;
            _value = value;
            _width = width;
            _height = height;
            _hp = hp;
            _mp = mp;

            _baseStats = baseStats.Where(x => x.Value != 0).ToArray();
            _reqStats = reqStats.Where(x => x.Value != 0).ToArray();

            // Make sure the ItemType is defined
            if (!type.IsDefined())
            {
                const string errmsg = "Invalid ItemType `{0}` for ItemTemplate ID `{1}`.";
                throw new InvalidCastException(string.Format(errmsg, type, id));
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, ID);
        }
    }
}