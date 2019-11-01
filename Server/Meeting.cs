using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    enum state { OPEN, SCHEDULED, CANCELED };

    public class Meeting
    {
        private static readonly DateTime ERRONEOUS_DATE = new DateTime(1900, 1, 31);

        private int minAttendees, version;
        private string coordinator, topic;

        private state state;

        private List<string> clients;
        private List<string> invitees;

        Tuple<Location, string, DateTime> chosenVenue;

        private List<Tuple<Location, DateTime>> proposedVenues;

        public Meeting(string topic, int minAttendees, List<Tuple<Location,DateTime>> slots, List<string> invitees, string client_address)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.coordinator = client_address;
            this.clients = new List<string>();
            this.clients.Add(client_address);
            this.proposedVenues = new List<Tuple<Location, DateTime>>(slots);
            this.invitees = invitees;            
            this.state = state.OPEN;
            this.version = 1;
        }

        public void Apply(List<Tuple<Location, DateTime>> slots, string client_address)
        {

            if (state == state.CANCELED)
            {
                throw new ServerCommunicationException("This Meeting is already CANCELED");
            }
            lock (this)
            {
                this.clients.Add(client_address);
                this.proposedVenues.Concat(slots);
                this.version += 1;
            }
        }

        public bool Schedule()
        {            
            Console.WriteLine(this.GetNumberOfCandidates());            
            Console.WriteLine("Entrou no Schedule");
            lock (this)
            {
                if (this.GetNumberOfCandidates() < MinAttendees)
                {
                    this.version++;
                    this.state = state.CANCELED;
                    
                    // TODO lancar excepcao remota                    
                    Console.WriteLine("\r\nEvent Canceled: " + topic);
                    Console.WriteLine("\r\nThe number of candidates was less than the number of minimum attendees...");

                    return false;

                } else
                {
                    bool result;                    
                    SortedDictionary<int, List<Tuple<Location, DateTime>>> dictionary;

                    dictionary = this.ScheduleOrganizeEntries();
                    this.chosenVenue = this.ScheduleSortEntries(dictionary);

                    if(chosenVenue!=null)
                    {
                        result = true;

                        this.version++;
                        this.state = state.SCHEDULED;

                        return result;

                    }
                    else
                    {
                        result = false; 

                        this.version++;
                        this.state = state.CANCELED;
                        // TODO lancar excepcao remota   
                        return result;
                    }
                    
                }
            }
        }

        private SortedDictionary<int, List<Tuple<Location, DateTime>>> ScheduleOrganizeEntries()
        {
            SortedDictionary<int, List<Tuple<Location, DateTime>>> dictionary = new SortedDictionary<int, List<Tuple<Location, DateTime>>>();
            List<Tuple<Location, DateTime>> explored = new List<Tuple<Location, DateTime>>(), tmpList;

            foreach (Tuple<Location, DateTime> outter_tuple in this.proposedVenues)
            {
                int count = 0;

                if(!explored.Contains(outter_tuple))
                {
                    foreach (Tuple<Location, DateTime> inner_tuple in this.proposedVenues)
                    {
                        if (outter_tuple.Equals(inner_tuple))
                        {
                            count++;
                        }
                    }
                    explored.Add(outter_tuple);

                    if (dictionary.ContainsKey(count))
                    {
                        tmpList = dictionary[count];
                        tmpList.Add(outter_tuple);
                        dictionary.Remove(count);
                        dictionary.Add(count, tmpList);
                    }
                    else
                    {
                        tmpList = new List<Tuple<Location, DateTime>>();
                        tmpList.Add(outter_tuple);
                        dictionary.Add(count, tmpList);
                    }
                }
                                
            }
            return dictionary;
        }

        private Tuple<Location, string, DateTime> ScheduleSortEntries(SortedDictionary<int, List<Tuple<Location, DateTime>>> dictionary)
        {

            bool end_iteration = false;
            string room_identifier; 
            
            Location sortEntriesLocation;
            DateTime sortEntriesDateTime;

            Tuple<Location, string, DateTime> tupleLocationRoomDate = null;

            List<Tuple<Location, DateTime>> sortEntriesList = new List<Tuple<Location, DateTime>>();

            foreach (int iter in dictionary.Keys.Reverse())
            {
                foreach(Tuple<Location, DateTime> tuple in dictionary[iter])
                {
                    sortEntriesLocation = tuple.Item1;
                    sortEntriesDateTime = tuple.Item2;

                    room_identifier = sortEntriesLocation.PickRoom(sortEntriesDateTime, this.clients.Count);

                    if(room_identifier != null)
                    {
                        tupleLocationRoomDate = new Tuple<Location, string, DateTime>(sortEntriesLocation, room_identifier, sortEntriesDateTime);
                        end_iteration = true;
                        break;                       
                    }
                }

                if(end_iteration)
                {
                    break;
                }
                    
            }

            return tupleLocationRoomDate;
        }
        public string Topic
        {
            get
            {
                return this.topic;
            }
        }

        public string Coordinator
        {
            get
            {
                return this.coordinator;
            }
        }

        public int MinAttendees
        {
            get
            {
                return this.minAttendees;
            }
        }

        public string GetSlotsData()
        {
            string slotsData = "";

            foreach (Tuple<Location, DateTime> s in this.proposedVenues)
            {
                slotsData += s.Item1.Name + ", " + s.Item2.ToString();
                slotsData += "\n";
            }

            return slotsData;
        }
        public int GetNumberOfCandidates()
        {
            return clients.Count();
        }
        public int GetVersion()
        {
            return this.version;
        }
        public List<string> GetSlots()
        {
            List<string> result = new List<String>();
            foreach (Tuple<Location, DateTime> t in this.proposedVenues)
            {
                string slot = "";
                slot += t.Item1.Name;
                slot += t.Item2.ToString();
                result.Add(slot);
            }

            return result;
        }
        public string GetState()
        {
            return this.state.ToString();
        }

        public List<string> GetInvitees()
        {
            return this.invitees;
        }

    }
}
