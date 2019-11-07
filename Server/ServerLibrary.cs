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
        int port;
        string ip, server;
        ServerCommunication communications;

        private List<Meeting> eventList = new List<Meeting>();
        private List<Location> knownLocations = new List<Location>();

        public ServerLibrary(string server, string ip, int port)
        {
            this.server = server;
            this.ip = ip;
            this.port = port;

            this.communications = new ServerCommunication(this, server, ip, port); ;

            Console.Write("Starting server remoting service... ");
            communications.Start();
            Console.WriteLine("Success!");
        }

        public void Create(string topic, int minAttendees, List<string> venues, List<string> invitees, string user)
        {
            Meeting m;
            List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(venues);

            lock (this)
            {
                m = new Meeting(topic, minAttendees, parsedSlots, invitees, user);
                eventList.Add(m);
            }
        }

        public void Join(string topic, List<string> slots, string user)
        {
            Meeting meeting = null;
            try
            {
                List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);
                meeting = GetMeeting(topic);
                meeting.Apply(parsedSlots, user);
            }
            catch (ServerCoreException sce)
            {
                throw sce;
            }
        }

        public void Close(String topic, string user)
        {
            GetMeeting(topic).Schedule(user);
            Console.Write("Please run a command to be run on the server: ");
        }
        
        public string GetServerIdentifier()
        {
            return server;
        }

        public string GetServerIP()
        {
            return ip;
        }

        public int GetServerPort()
        {
            return port;
        }

        public void AddMeeting(Meeting meeting)
        {
            this.eventList.Add(meeting);
        }

        private Meeting GetMeeting(string topic)
        {
            foreach (Meeting m in this.eventList)
            {
                if (m.Topic == topic)
                {
                    return m;
                }
            }
            throw new ServerCoreException(ErrorCodes.NONEXISTENT_MEETING);
        }

        public void AddLocation(Location location)
        {
            this.knownLocations.Add(location);
        }

        public List<Meeting> GetEventList()
        {
            return this.eventList;
        }

        public List<Location> GetKnownLocations()
        {
            return this.knownLocations;
        }

        public Tuple<Location, DateTime> ParseSlot(String slot)
        {
            Location location = null;
            String[] data = slot.Split(',');
            DateTime date = DateTime.Parse(data[1]);

            foreach (Location l in knownLocations)
            {
                if (l.Name == data[0])
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
            List<Tuple<Location, DateTime>> parsedSlots = new List<Tuple<Location, DateTime>>();
            foreach (string s in slots)
            {
                parsedSlots.Add(ParseSlot(s));
            }
            return parsedSlots;
        }
    }
}
