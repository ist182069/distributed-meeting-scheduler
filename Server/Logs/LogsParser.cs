using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Logs
{
    class LogsParser
    {
        public Tuple<string, string, int, List<string>, List<string>, string> ParseEntry(string json_entry)
        {
            Console.WriteLine(json_entry);            
            JObject jo = JObject.Parse(json_entry);
            string operation = jo["Operation"].ToString();

            string op;
            string meeting_topic;
            int min_attendees;
            List<string> slots;
            List<string> invitees;
            string client_identifier;

            switch (operation)
            {
                case "Create":
                    Create create = JsonConvert.DeserializeObject<Create>(json_entry);

                    op = create.Operation;
                    meeting_topic = create.MeetingTopic;
                    min_attendees = create.MinAttendees;
                    slots = create.Slots;
                    invitees = create.Invitees;
                    client_identifier = create.ClientIdentifier;

                    return new Tuple<string, string, int, List<string>, List<string>, string>(op, meeting_topic, min_attendees, slots, invitees, client_identifier);
                case "Join":
                    Join join = JsonConvert.DeserializeObject<Join>(json_entry);

                    op = join.Operation;
                    meeting_topic = join.MeetingTopic;
                    min_attendees = -69;
                    slots = join.Slots;
                    invitees = null;
                    client_identifier = join.ClientIdentifier;

                    return new Tuple<string, string, int, List<string>, List<string>, string>(op, meeting_topic, min_attendees, slots, invitees, client_identifier);
                case "Close":
                    Close close = JsonConvert.DeserializeObject<Close>(json_entry);

                    op = close.Operation;
                    meeting_topic = close.MeetingTopic;
                    min_attendees = -69;
                    slots = null;
                    invitees = null;
                    client_identifier = close.ClientIdentifier;

                    return new Tuple<string, string, int, List<string>, List<string>, string>(op, meeting_topic, min_attendees, slots, invitees, client_identifier);
                default:
                    throw new Exception("Not supposed to happen");
            }
        }
        public string Create_ParseJSON(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier)
        {
            Create create = new Create(meeting_topic, min_attendees, slots, invitees, client_identifier);
            string json_create = JsonConvert.SerializeObject(create);

            return json_create;
        }

        private void Create_ParseMeeting(string json_string)
        {
            Create create = JsonConvert.DeserializeObject<Create>(json_string);
        }

        public string Join_ParseJSON(string meeting_topic, List<string> slots, string client_identifier)
        {
            Join join = new Join(meeting_topic, client_identifier, slots);
            string json_create = JsonConvert.SerializeObject(join);

            return json_create;
        }

        private void Join_ParseMeeting(string json_string)
        {
            Join join = JsonConvert.DeserializeObject<Join>(json_string);
        }

        public string Close_ParseJSON(string meeting_topic, string client_identifier)
        {
            Close close = new Close(meeting_topic, client_identifier);
            string json_create = JsonConvert.SerializeObject(close);

            return json_create;
        }

        private void Close_ParseMeeting(string json_string)
        {
            Close close = JsonConvert.DeserializeObject<Close>(json_string);
        }
    }

    class Create
    {
        string operation, meeting_topic, client_identifier;
        int min_attendees;
        List<string> slots, invitees;

        public Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier)
        {
            this.operation = "Create";
            this.meeting_topic = meeting_topic;
            this.min_attendees = min_attendees;
            this.slots = slots;
            this.invitees = invitees;
            this.client_identifier = client_identifier;
        }

        public string Operation
        {
            get
            {
                return this.operation;
            } 
            set
            {
                this.operation = value;
            }
        }
        public string MeetingTopic 
        {
            get
            {
                return this.meeting_topic;
            }
            set
            {
                this.meeting_topic = value;
            }
        }

        public string ClientIdentifier
        {
            get
            {
                return this.client_identifier;
            }
            set
            {
                this.client_identifier = value;
            }
        }

        public int MinAttendees
        {
            get
            {
                return this.min_attendees;
            } 
            set
            {
                this.min_attendees = value;
            }
        }

        public List<string> Slots
        {
            get
            {
                return this.slots;
            }
            set
            {
                this.slots = value;
            }
        }

        public List<string> Invitees
        {
            get
            {
                return this.invitees;
            }
            set
            {
                this.invitees = value;
            }
        }
    }

    class Join
    {
        string operation, client_identifier, meeting_topic;
        List<string> slots;
        public Join(string client_identifier, string meeting_topic, List<string> slots)
        {
            this.operation = "Join";
            this.client_identifier = client_identifier;
            this.meeting_topic = meeting_topic;
            this.slots = slots;
        }

        public string Operation
        {
            get
            {
                return this.operation;
            } 
            set
            {
                this.operation = value;
            }
        }
        public string MeetingTopic
        {
            get
            {
                return this.meeting_topic;
            }
            set
            {
                this.meeting_topic = value;
            }
        }

        public string ClientIdentifier
        {
            get
            {
                return this.client_identifier;
            }
            set
            {
                this.client_identifier = value;
            }
        }

        public List<string> Slots
        {
            get
            {
                return this.slots;
            }
            set
            {
                this.slots = value;
            }
        }
    }

    class Close
    {
        string operation, client_identifier, meeting_topic;
        public Close(string meeting_topic, string client_identifier)
        {
            this.operation = "Close";
            this.meeting_topic = meeting_topic;
            this.client_identifier = client_identifier;            
        }

        public string Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }

        public string MeetingTopic
        {
            get
            {
                return this.meeting_topic;
            }
            set
            {
                this.meeting_topic = value;
            }
        }

        public string ClientIdentifier
        {
            get
            {
                return this.client_identifier;
            }
            set
            {
                this.client_identifier = value;
            }
        }
    }
}
