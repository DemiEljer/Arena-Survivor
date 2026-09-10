using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Standard.Collections
{
    public class CollectionIndexCyclicalIterator
    {
        public int CurrentIndex { get; private set; } = 0;

        public int GetNextIndex(int collectionSize)
        {
            if (collectionSize <= 0)
            {
                return -1;
            }
            else
            {
                int returnIndex = CurrentIndex;

                CurrentIndex = (CurrentIndex + 1) % collectionSize;

                return returnIndex;
            }
        }

        public TValue GetNextArrayElement<TValue>(TValue[] array)
        {
            if (array is null || array.Length == 0)
            {
                return default(TValue);
            }
            else
            {
                return array[GetNextIndex(array.Length)];
            }
        }
    }
}
