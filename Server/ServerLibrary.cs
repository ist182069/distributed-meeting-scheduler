using MSDAD.Library;
using MSDAD.Server.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MSDAD.Server
{
    class ServerLibrary
    {
        int server_port;
        string server_ip, server_identifier, server_remoting;
        ServerCommunication server_communication;

        private List<Meeting> event_list = new List<Meeting>();
        private List<Location> known_locations = new List<Location>();

        public ServerLibrary(string server_identifier, string server_remoting, string server_ip, int server_port)
        {
            this.server_identifier = server_identifier;
            this.server_ip = server_ip;
            this.server_port = server_port;
            this.server_remoting = server_remoting;

            this.server_communication = new ServerCommunication(this); ;

            Console.Write("Starting server remoting service... ");
            server_communication.Start();
            Console.WriteLine("Success!");
        }

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier)
        {
            Meeting m;
            List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);

            lock (this)
            {
                m = new Meeting(meeting_topic, min_attendees, parsedSlots, invitees, client_identifier);
                event_list.Add(m);
            }
        }

        public void Join(string meeting_topic, List<string> slots, string client_identifier)
        {
            Meeting meeting = null;
            try
            {
                List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);
                meeting = GetMeeting(meeting_topic);
                meeting.Apply(parsedSlots, client_identifier);
            }
            catch (ServerCoreException sce)
            {
                throw sce;
            }
        }

        public void Close(String meeting_topic, string client_identifier)
        {
            GetMeeting(meeting_topic).Schedule(client_identifier);
            Console.Write("Please run a command to be run on the server: ");
        }
        
        public string ServerIdentifier
        {
            get{
                return server_identifier;
            }
        }

        public string ServerIP
        {
            get
            {
                return server_ip;
            }
        }

        public string ServerRemoting
        {
            get
            {
                return server_remoting;
            }
        }

        public int ServerPort
        {
            get
            {
                return server_port;
            }
        }

        public void AddMeeting(Meeting meeting)
        {
            this.event_list.Add(meeting);
        }

        private Meeting GetMeeting(string meeting_topic)
        {
            foreach (Meeting m in this.event_list)
            {
                if (m.Topic == meeting_topic)
                {
                    return m;
                }
            }
            throw new ServerCoreException(ErrorCodes.NONEXISTENT_MEETING);
        }

        public void AddLocation(Location location)
        {
            //agora tem um check que adiciona um quarto a localizacao se ela ja existir
            Location existingLocation = null;
            List<Room> rooms;
                          
            foreach(Location l in this.known_locations)
            {
                if(l.Name.Equals(location.Name))
                {
                    existingLocation = l;           
                }
            }

            if(existingLocation!=null)
            {
                rooms = location.GetList();

                foreach(Room room in rooms)
                {
                    existingLocation.Add(room);
                }
            }
            else
            {
                this.known_locations.Add(location);
            }
        }

        public List<Meeting> GetEventList()
        {
            return this.event_list;
        }

        public List<Location> GetKnownLocations()
        {
            return this.known_locations;
        }

        public Tuple<Location, DateTime> ParseSlot(String slot)
        {
            Location location = null;
            String[] date_string = slot.Split(',');
            DateTime date = DateTime.Parse(date_string[1]);

            foreach (Location l in known_locations)
            {
                if (l.Name == date_string[0])
                {
                    location = l;
                }
            }
            if (location == null)
            {
                throw new ServerCoreException(ErrorCodes.NOT_A_LOCATION);
            }

            return new Tuple<Location, DateTime>(location, date);
        }

        public List<Tuple<Location, DateTime>> ListOfParsedSlots(List<string> slots)
        {
            List<Tuple<Location, DateTime>> parsed_slots = new List<Tuple<Location, DateTime>>();
            foreach (string s in slots)
            {
                parsed_slots.Add(ParseSlot(s));
            }
            return parsed_slots;
        }
    }
}
