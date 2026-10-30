using Assets.Project.Code.Scripts.Agents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Standard.Collections
{
    public class CollectionMapper<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> _MapperCollection { get; } = new();

        public TValue Get(TKey key)
        {
            if (key is not null)
            {
                return _MapperCollection.GetValueOrDefault(key);
            }
            else
            {
                return default(TValue);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key is not null)
            {
                _MapperCollection.AddOrUpdate(key, (_) => value, (_, _) => value);
            }
        }

        internal void Remove(TKey key)
        {
            if (key is not null)
            {
                TValue value;

                _MapperCollection.Remove(key, out value);
            }
        }
    }
}
