using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    public class Room 
    {
        private string identifier;
        private int capacity;
        private List<DateTime> reservations = new List<DateTime>();

        public Room(String identifier, int capacity)
        {
            this.identifier = identifier;
            this.capacity = capacity;            
        }

        public string Identifier
        {
            get
            {
                return this.identifier;
            }
            set
            {
                this.identifier = value;
            }
        }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                this.capacity = value;
            }
        }
        
        public void Reserve(DateTime dateTime)
        {
            reservations.Add(dateTime);
        }   
        
        public bool Vacancy(DateTime dateTime)
        {
            bool vacated;

            if(reservations.Contains(dateTime))
            {
                vacated = false;
            } else
            {
                vacated = true;
            }

            return vacated;
        }

        public List<DateTime> ReservedDates             
        {
            get
            {
                return this.reservations;
            }     
            set
            {
                this.reservations = value;
            }

        }
    }
}
