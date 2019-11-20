using MSDAD.PCS.Exceptions;
using MSDAD.PCS.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS
{
    class PCSLibrary
    {
        string[] words;

        Dictionary<string, LocationXML> locationDictionary; 
        Dictionary<string, Tuple<string, Process>> serverDictionary;
        Dictionary<string, Tuple<string, Process>> clientDictionary;

        public PCSLibrary()
        {
            this.locationDictionary = new Dictionary<string, LocationXML>();
            this.serverDictionary = new Dictionary<string, Tuple<string, Process>>();
            this.clientDictionary = new Dictionary<string, Tuple<string, Process>>();
        }      

        public string[] GetWords()
        {
            return this.words;
        }

        public void SetWords(string[] words)
        {
            this.words = words;
        }

        public Dictionary<string, LocationXML> GetLocationDictionary()
        {
            return this.locationDictionary;
        }

        public void AddLocationXML(string location_name, int room_capacity, string room_name)
        {
           

            if(this.locationDictionary.ContainsKey(location_name))
            {
                LocationXML locationXML = this.locationDictionary[location_name];

                RoomXML roomXML = new RoomXML();
                roomXML.Name = room_name;
                roomXML.Capacity = room_capacity;

                if (!locationXML.RoomViews.Contains(roomXML))
                {
                    locationXML.RoomViews.Add(roomXML);
                    this.locationDictionary[location_name] = locationXML;
                }
                else
                {
                    throw new PCSException("Error: You cannot insert the same room for location \"" + location_name + "\" more than once!");
                }
                
            }
            else
            {
                LocationXML locationXML = new LocationXML();
                locationXML.Name = location_name;

                RoomXML roomXML = new RoomXML();
                roomXML.Name = room_name;
                roomXML.Capacity = room_capacity;

                locationXML.RoomViews.Add(roomXML);

                this.locationDictionary.Add(location_name, locationXML);
            }            
        }

        public Dictionary<string, Tuple<string, Process>> GetServerDictionary()
        {
            return this.serverDictionary;
        }

        public void AddKeyValueToServerDictionary(string server_identifier, Tuple<string, Process> urlProcessTuple)
        {
            this.serverDictionary.Add(server_identifier, urlProcessTuple);
        }

        public Dictionary<string, Tuple<string, Process>> GetClientDictionary()
        {
            return this.clientDictionary;
        }

        public void AddKeyValueToClientDictionary(string client_identifier, Tuple<string, Process> urlProcessTuple)
        {
            this.clientDictionary.Add(client_identifier, urlProcessTuple);
        }
    }
}
