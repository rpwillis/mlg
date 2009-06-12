using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace MLG2007.Helper.CalendarStore
{
    /// <summary>A collection of calendars.</summary>
    public class CalendarCollection : System.Collections.CollectionBase, IEnumerable, IEnumerator 
    {
        //System.Collections.ArrayList m_objects;
        private int index = -1;

        /// <summary>Initializes a new instance of <see cref="CalendarCollection"/>.</summary>
        public CalendarCollection()
        {
           this.index = -1;
        }

        /// <summary>Adds a calendar to the collection.</summary>
        public void Add(Calendar calendar)
        {
            this.List.Add(calendar); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        /// <summary>The current object in the iteration.</summary>
        public Object Current
        {
            get
            {
                return this.List[index];
            }
        }

        /// <summary>Move to the next calendar.</summary>
        public bool MoveNext() 
        {
            this.index++;
            return (this.index < this.List.Count);
        }

        /// <summary>Resets the iterator.</summary>
        public void Reset()
        {
            this.index = -1;
        }

        /// <summary>The indexer for the collection.</summary>
        public Calendar  this[int index]
        {
            get { return (Calendar)this.List[index]; }
            set { this.List[index] = value; }
        }
    }
}
