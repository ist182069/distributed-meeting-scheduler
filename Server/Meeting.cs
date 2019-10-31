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

        private Dictionary<string, List<Tuple<Location, DateTime>>> candidates;

        private List<Tuple<Location, DateTime>> slots;
        private List<string> invitees;

        private Tuple<DateTime, string> dateTimeRoomTuple;
    
        public Meeting(string topic, int minAttendees, List<Tuple<Location,DateTime>> slots, List<string> invitees, string client_address)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
            this.coordinator = client_address;
            this.invitees = invitees;
            this.candidates = new Dictionary<string, List<Tuple<Location, DateTime>>>();
            this.candidates[client_address] = this.slots;
            this.state = state.OPEN;
            this.version = 1;
        }


        public Boolean IsInvited(string client_address)
        {
            if(this.invitees == null)
            {
                return true;
            }

            return this.invitees.Contains(client_address);
        }

        public Boolean IsCandidate(string client_address)
        {
            return candidates.ContainsKey(client_address);
        }

        public void Apply(List<Tuple<Location, DateTime>> slots, string client_address)
        {

            if (state == state.CANCELED)
            {
                throw new ServerCommunicationException("This Meeting is already CANCELED");
            }
            lock (this)
            {
                this.candidates.Add(client_address, slots);
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

                    DateTime chosenDate;
                    Dictionary<Location, List<Tuple<DateTime, int>>> locationsDictionary;

                    locationsDictionary = this.ScheduleSortEntries();

                    chosenDate = this.ScheduleChooseEntries(locationsDictionary);                   

                    if(chosenDate!=ERRONEOUS_DATE)
                    {
                        this.version++;
                        this.state = state.SCHEDULED;

                        dateTimeRoomTuple = new Tuple<DateTime, string>(chosenDate, null);
                        // TODO escolhe um room                        

                        Console.WriteLine("\r\nEvent Scheduled: " + topic);

                        return true;

                    }
                    else
                    {
                        this.version++;
                        this.state = state.CANCELED;
                        // TODO lancar excepcao remota   
                        return false;
                    }
                    
                }
            }
        }

        private Dictionary<Location, List<Tuple<DateTime, int>>> ScheduleSortEntries()
        {
            List<Tuple<Location, DateTime>> tuplesListIter;
            Location locationIter;
            DateTime dateTimeIter;

            Tuple<DateTime, int> tuple;
            Dictionary<Location, List<Tuple<DateTime, int>>> locationsDictionary;

            locationsDictionary = new Dictionary<Location, List<Tuple<DateTime, int>>>();

            foreach (KeyValuePair<string, List<Tuple<Location, DateTime>>> entry in this.candidates)
            {
                
                tuplesListIter = entry.Value;

                foreach (Tuple<Location, DateTime> tupleLocDateIter in tuplesListIter)
                {
                    locationIter = tupleLocDateIter.Item1;
                    dateTimeIter = tupleLocDateIter.Item2;

                    if (!locationsDictionary.ContainsKey(locationIter))
                    {
                        List<Tuple<DateTime, int>> newList = new List<Tuple<DateTime, int>>();
                        Tuple<DateTime, int> tmpTuple = new Tuple<DateTime, int>(dateTimeIter, 1);

                        newList.Add(tmpTuple);

                        locationsDictionary.Add(locationIter, newList);
                    }
                    else
                    {
                        bool isNewDate = true;
                        List<Tuple<DateTime, int>> iterList, tmpList;

                        iterList = locationsDictionary[locationIter];
                        tmpList = new List<Tuple<DateTime, int>>(iterList);

                        foreach (Tuple<DateTime, int> tupleDateIntIter in iterList)
                        {
                            int dateCount = tupleDateIntIter.Item2;
                            DateTime tupleDateTime = tupleDateIntIter.Item1;

                            if (dateTimeIter == tupleDateTime)
                            {
                                dateCount++;
                                tmpList.Remove(tupleDateIntIter);                                
                                tmpList.Add(new Tuple<DateTime, int>(dateTimeIter, dateCount));
                                locationsDictionary[locationIter] = tmpList;
                                isNewDate = false;
                            }

                        }

                        if (isNewDate)
                        {
                            tmpList.Add(new Tuple<DateTime, int>(dateTimeIter, 1));
                            locationsDictionary[locationIter] = tmpList;
                        }
                    }
                }

            }

            return locationsDictionary;
        }

        private DateTime ScheduleChooseEntries(Dictionary<Location, List<Tuple<DateTime, int>>> dictionary)
        {            
            int biggestCount = 0;
            DateTime mostPopularDateTime = new DateTime(1900, 1, 31);

            foreach (KeyValuePair<Location, List<Tuple<DateTime, int>>> listTupleIter in dictionary)
            {
                Console.WriteLine("###");
                Console.WriteLine("Location: ");
                Console.WriteLine(listTupleIter.Key.Name);
                Console.WriteLine("DateTimes: ");
                
                foreach(Tuple<DateTime, int> tupleDateTime in listTupleIter.Value)
                {
                    int currentCount = tupleDateTime.Item2;

                    Console.WriteLine(tupleDateTime.Item1.ToString());
                    Console.WriteLine(tupleDateTime.Item2.ToString());

                    if(biggestCount < currentCount)
                    {
                        mostPopularDateTime = tupleDateTime.Item1;
                        biggestCount = currentCount;
                    }

                }

                Console.WriteLine("###");                
            }

            // TODO neste momento ele apenas escolhe o local mais popular. Deveriamos ter uma coisa, que em caso de empate ele escolhe o Local com a data mais popular
            return mostPopularDateTime;
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

        public string getSlotsData()
        {
            string slotsData = "";

            foreach (Tuple<Location, DateTime> s in this.slots)
            {
                slotsData += s.Item1.Name + ", " + s.Item2.ToString();
                slotsData += "\n";
            }

            return slotsData;
        }
        public int GetNumberOfCandidates()
        {
            return candidates.Count();
        }
        public int getVersion()
        {
            return this.version;
        }
        public List<string> getSlots()
        {
            List<string> result = new List<String>();
            foreach (Tuple<Location, DateTime> t in slots)
            {
                string slot = "";
                slot += t.Item1.Name;
                slot += t.Item2.ToString();
                result.Add(slot);
            }

            return result;
        }
        public string getState()
        {
            return this.state.ToString();
        }

        public List<string> GetInvitees()
        {
            return this.invitees;
        }

    }
}
