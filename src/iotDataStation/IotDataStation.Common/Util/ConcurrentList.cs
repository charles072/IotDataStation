using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDataStation.Common.Util
{
    public class ConcurrentList<T> : IList<T>
    {
        private IList<T> _internalList;
        private readonly object lockObject = new object();

        public ConcurrentList()
        {
            _internalList = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return LockInternalListAndQuery(i => i.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return LockInternalListAndQuery(i => i.GetEnumerator());
        }

        public void Add(T item)
        {
            LockInternalListAndCommand(i => i.Add(item));
        }

        public void Clear()
        {
            LockInternalListAndCommand(i => i.Clear());
        }

        public bool Contains(T item)
        {
            return LockInternalListAndQuery(i => i.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            LockInternalListAndCommand(i => i.CopyTo(array, arrayIndex));
        }

        public bool Remove(T item)
        {
            return LockInternalListAndQuery(i => i.Remove(item));
        }

        public int Count
        {
            get { return LockInternalListAndQuery(i => i.Count()); }
        }

        public bool IsReadOnly => false;
        public int IndexOf(T item)
        {
            return LockInternalListAndQuery(i => i.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            LockInternalListAndCommand(i => i.Insert(index, item));
        }

        public void RemoveAt(int index)
        {
            LockInternalListAndCommand(i => i.RemoveAt(index));
        }

        public T this[int index]
        {
            get { return LockInternalListAndGet(l => l[index]);}
            set { LockInternalListAndCommand(l => l[index] = value); }
        }

        protected virtual T LockInternalListAndGet(Func<IList<T>, T> func)
        {
            lock (lockObject)
            {
                return func(_internalList);
            }
        }
        protected virtual  void LockInternalListAndCommand(Action<IList<T>> action)
        {
            lock (lockObject)
            {
                action(_internalList);
            }
        }
        protected virtual TObject LockInternalListAndQuery<TObject>(Func<IList<T>, TObject> query)
        {
            lock (lockObject)
            {
                return query(_internalList);
            }
        }
    }
}
