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
    }
}
