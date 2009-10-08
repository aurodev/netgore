using System;
using System.Linq;
using NetGore;

namespace NetGore.Collections
{
    public class DArrayModifyEventArgs<T> : EventArgs
    {
        public readonly int Index;
        public readonly T Item;

        public DArrayModifyEventArgs(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}