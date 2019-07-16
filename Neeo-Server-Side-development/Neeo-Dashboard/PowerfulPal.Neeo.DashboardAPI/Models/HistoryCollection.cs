using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class HistoryCollection : IList<History>
    {
        private IList<History> _list = new List<History>();


        public HistoryCollection(DateTime lastSyncTime)
        {
            for (int i = 1; i <= 7; i++)
            {
                var date = lastSyncTime.Subtract(new TimeSpan(i, 0, 0, 0));
                var item = new History();
                item.Date = date.ToString("d");
                item.Caption = date.ToString("dd MMM yyyy");
                //if (i == 0)
                //{
                //    item.Caption = DateTime.Compare(date.Date, DateTime.UtcNow.Date) == 0 ? "Today" : "Yesterday";
                //}
                //else if (i == 1)
                //{
                //    item.Caption = DateTime.UtcNow.Date.Subtract(date.Date).Days == 1 ? "Yesterday" : date.ToString("dd MMM yyyy");
                //}
                //else
                //{
                //    item.Caption = date.ToString("dd MMM yyyy");
                //}
                _list.Add(item);
            }
        }

        public int IndexOf(History item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, History item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public History this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(History item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(History item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(History[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _list.IsReadOnly;
            }
        }

        public bool Remove(History item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<History> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}