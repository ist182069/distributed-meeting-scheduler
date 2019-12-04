using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    public class Location
    {
        private string name;
        private List<Room> rooms;

        public Location(string name)
        {
            this.name = name;
            this.rooms = new List<Room>();
        }

        public Location(string name, List<Room> rooms)
        {
            this.name = name;
            this.rooms = rooms;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public List<Room> Rooms
        {
            get
            {
                return this.rooms;
            }            
            set
            {
                this.rooms = value;
            }
        }

        public void Add(Room room)
        {
            if (rooms.Contains(room))
            {
                return;
            }
            this.rooms.Add(room);
        }

        public void Pick(Room room, DateTime dateTime)
        {
            foreach(Room roomIter in this.rooms)
            {
                if(roomIter.Equals(room))
                {
                    roomIter.Reserve(dateTime);
                    break;
                }
            }
        }

        public Room Select(DateTime dateTime, int clients_count, int min_attendees)
        {
            Room resultRoom = null; 

            foreach(Room room in this.rooms)
            {
                if(room.Vacancy(dateTime))
                {
                    if(room.Capacity >= min_attendees)
                    {
                        
                        if (resultRoom == null || room.Capacity >= resultRoom.Capacity)
                        {
                            resultRoom = room;

                            if (clients_count <= room.Capacity)
                            {                                
                                break;
                            }
                        }

                    }
                    
                   
                }
            }

            return resultRoom;
        }
    }
}
