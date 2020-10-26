using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiscUtil.Conversion
{
    public class Optional<T> : IEnumerable<T>
    {
        private readonly T[] data;

        private Optional(T[] data)
        {
            this.data = data;
        }

        public static Optional<T> Create(T element)
        {
            return new Optional<T>(new[] { element });
        }

        public static Optional<T> CreateEmpty()
        {
            return new Optional<T>(new T[0]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.data).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void SetIfPresent(ref T variable)
        {
            if (data.Length > 0)
            {
                variable = data[0];
            }
        }

        public T OrElse(T value)
        {
            if (data.Length > 0)
            {
                return data[0];
            }
            return value;
        }

        public void IfPresent(Action<T> action)
        {
            if (data.Length > 0)
            {
                action(data[0]);
            }
        }
    }
}
