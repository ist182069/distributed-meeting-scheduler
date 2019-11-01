using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    public class Location
    {
        private String name;
        private List<Room> rooms;

        public Location(String name)
        {
            this.name = name;
            this.rooms = new List<Room>();
        }

        public Location(String name, List<Room> rooms)
        {
            this.name = name;
            this.rooms = rooms;
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public List<Room> getList()
        {
            return this.rooms;
        }

        public void AddRoom(Room room)
        {
            if (rooms.Contains(room))
            {
                return;
            }
            this.rooms.Add(room);
        }

        public string PickRoom(DateTime dateTime, int clients_count)
        {
            
            List<Room> proposedRooms;

            proposedRooms = this.PickRoomSort(dateTime, clients_count);
            string room_identifier = this.PickRoomChoose(dateTime, proposedRooms);

            return room_identifier;
        }

        private List<Room> PickRoomSort(DateTime dateTime, int clients_count)
        {
            int current_difference, minimum_difference = int.MaxValue, room_capacity;
            List<Room> proposedRooms = new List<Room>();

            foreach (Room roomIter in this.rooms)
            {
                room_capacity = roomIter.Capacity;

                current_difference = room_capacity - clients_count;

                if (current_difference < 0)
                {
                    continue;
                }
                else if (current_difference < minimum_difference)
                {
                    minimum_difference = current_difference;
                    proposedRooms.Insert(0, roomIter);
                }
                else
                {
                    proposedRooms.Add(roomIter);
                }
            }

            return proposedRooms;
        }

        private string PickRoomChoose(DateTime dateTime, List<Room> proposedRooms)
        {
            bool reserved;
            string room_identifier = null;

            foreach(Room roomIter in proposedRooms)
            {                
                Console.WriteLine("proposedRooms");
                Console.WriteLine(roomIter.Identifier);
                reserved = roomIter.ReserveRoom(dateTime);

                if(!reserved)
                {
                    room_identifier = roomIter.Identifier;
                    break;
                }

            }

            return room_identifier;
        }
    }
}
