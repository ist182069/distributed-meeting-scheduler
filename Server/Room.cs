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
        private List<DateTime> reservations = new List<DateTime>();

        public Room(String identifier, int capacity)
        {
            this.identifier = identifier;
            this.capacity = capacity;            
        }

        public String Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }
        }
        
        public bool ReserveRoom(DateTime dateTime)
        {
            bool reserved;

            if(reservations.Contains(dateTime))
            {
                reserved = true;
            } else
            {
                reserved = false;
                reservations.Add(dateTime);
            }

            return reserved;
        }
    }
}
