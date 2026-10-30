using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Standard.Collections
{
    public class CollectionUniqIndexer
    {
        private ulong _NextIndex { get; set; } = 0;

        private object _Locker { get; } = new();

        public ulong GetNextIndex()
        {
            lock (_Locker)
            {
                ulong index = _NextIndex;

                _NextIndex++;

                return index;
            }
        }
    }
}
