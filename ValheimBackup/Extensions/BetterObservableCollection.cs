using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.Extensions
{
    public class ItemNotFoundException : ArgumentException
    {
        public object Item;

        public ItemNotFoundException(string message) : base(message)
        { }
        public ItemNotFoundException(string message, object item) : base(message)
        {
            Item = item;
        }
    }

    public class BetterObservableCollection<T> : ObservableCollection<T>
    {
        public BetterObservableCollection() : base()
        { }

        public BetterObservableCollection(List<T> list) : base(list)
        { }

        public void Replace(T original, T replacement)
        {
            int i = this.IndexOf(original);

            if (i < 0)
            {
                throw new ItemNotFoundException("Cannot replace. Original object not found in collection.", original);
            }

            this.SetItem(i, replacement);
        }
    }
}
