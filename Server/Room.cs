using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    public class Room 
    {
        private String identifier;
        private int capacity;
        private bool reserved;

        public Room(String identifier, int capacity)
        {
            this.identifier = identifier;
            this.capacity = capacity;
            this.reserved = false;
        }

        public String Identifier
        {
            get
            {
                return this.Identifier;
            }
        }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }
        }

        public bool Reserved
        {
            get
            {
                return this.reserved;
            }
        }

    }
}
