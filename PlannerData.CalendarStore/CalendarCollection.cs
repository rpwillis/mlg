using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace MLG2007.Helper.CalendarStore
{
    public class CalendarCollection : System.Collections.CollectionBase, IEnumerable, IEnumerator 
    {
        //System.Collections.ArrayList m_objects;
        private int index = -1;

        public CalendarCollection()
        {
           this.index = -1;
        }

        public void Add(Calendar calendar)
        {
            this.List.Add(calendar); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Object Current
        {
            get
            {
                return this.List[index];
            }
        }

        public bool MoveNext() 
        {
            this.index++;
            return (this.index < this.List.Count);
        }

        public void Reset()
        {
            this.index = -1;
        }
        public Calendar  this[int index]
        {
            get { return (Calendar)this.List[index]; }
            set { this.List[index] = value; }
        }
    }
}
