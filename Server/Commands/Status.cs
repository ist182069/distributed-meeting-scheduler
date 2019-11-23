using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands
{
    class Status : Command
    {
        public Status(ref ServerLibrary server_library) : base(ref server_library)
        {            
        }
        public override object Execute()
        {
            string client_identifier;
            string client_url;

            List<string> invitees;

            List<Location> locations;
            List<Meeting> meetings;
            List<Room> rooms;
            List<DateTime> reserved_dates;

            Dictionary<string, string> client_dictionary;

            locations = server_library.GetKnownLocations();
            client_dictionary = server_library.GetClients();
            meetings = server_library.GetEventList();

            foreach (Location location in locations)
            {
                Console.Write("Location: ");
                Console.WriteLine(location.Name);

                rooms = location.GetList();

                foreach (Room room in rooms)
                {
                    Console.WriteLine("  Room: " + room.Identifier);
                    Console.WriteLine("  Capacity: " + room.Capacity);
                    Console.WriteLine("  Dates Reserved: ");
                    reserved_dates = room.GetReservedDates();

                    foreach (DateTime dateTime in reserved_dates)
                    {
                        Console.WriteLine("    " + dateTime.ToString("yyyy-MMMM-dd"));
                    }
                }
            }


            foreach (KeyValuePair<string, string> client_pair in client_dictionary)
            {
                client_identifier = client_pair.Key;
                client_url = client_pair.Value;
                Console.Write("Client: ");
                Console.WriteLine(client_identifier + " / " + client_url);
            }
            foreach (Meeting meeting in meetings)
            {
                Console.Write("Meeting: ");
                Console.WriteLine(meeting.Topic);
                Console.WriteLine("Minimum atteendees: " + meeting.MinAttendees);
                Console.WriteLine("State: " + meeting.State);
                Console.WriteLine("Version: " + meeting.Version);
                Console.WriteLine("Coordinator: " + meeting.Coordinator);
                invitees = meeting.GetInvitees();

                if (invitees != null)
                {
                    foreach (string invitee in invitees)
                    {
                        Console.WriteLine("  Invitees:");
                        Console.WriteLine("  " + invitee);
                    }
                }

                Console.WriteLine("Final Slot: " + meeting.FinalSlot);
            }

            return null;
        }
    }
}
