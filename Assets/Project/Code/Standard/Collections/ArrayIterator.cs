using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Standard.Collections
{
    public class ArrayIterator<TArrayElement> : IEnumerable<TArrayElement>
    {
        public TArrayElement[] Array { get; }

        private int _Index { get; set; } = 0;

        public int Index 
        {
            get => _Index;
            set => _Index = Math.Clamp(value, 0, Array.Length - 1);
        }

        public int Length => Array.Length;

        public TArrayElement Prev => _Index > 0 ? Array[_Index - 1] : Array[_Index];

        public TArrayElement Current => Array[_Index];

        public TArrayElement Next => _Index < (Array.Length - 1) ? Array[_Index + 1] : Array[_Index];

        public TArrayElement First => Array[0];

        public TArrayElement Last => Array[Array.Length - 1];

        public bool IsEndAchived => _Index == (Array.Length - 1);

        public ArrayIterator(TArrayElement[] array)
        {
            Array = array;
        }

        public ArrayIterator()
        {
        }

        public bool MoveNext()
        {
            if (_Index < (Array.Length - 1))
            {
                _Index++;

                return true;
            }

            return false;
        }

        public bool MovePrev()
        {
            if (_Index > 0)
            {
                _Index--;

                return true;
            }

            return false;
        }

        public void Reset() => _Index = 0;

        public IEnumerable<TArrayElement> TakeLeft(int offset = 0)
        {
            for (int i = _Index + 1 + offset; i < Length; i++)
            {
                yield return Array[i];
            }
        }

        public IEnumerator<TArrayElement> GetEnumerator()
        {
            foreach (var element in Array)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
