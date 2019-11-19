using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands
{
    class AddRoom : Command
    {
        private string[] split_command;

        public AddRoom(ref ServerLibrary server_library, string[] split_command) : base(ref server_library)
        {
            this.split_command = split_command;
        }
        public override object Execute()
        {
            int capacity;
            string location_name, room_name;
            Location location;
            Room room;            

            location_name = split_command[1];
            capacity = Int32.Parse(split_command[2]);
            room_name = split_command[3];

            room = new Room(room_name, capacity);
            location = new Location(location_name);
            location.Add(room);

            base.server_library.AddLocation(location);

            return null;
        }
    }
}
