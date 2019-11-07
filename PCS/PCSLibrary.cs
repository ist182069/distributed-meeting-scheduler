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

        List<Tuple<string, int, string>> tuples;
        Dictionary<string, Process> serverDictionary;
        Dictionary<string, Process> clientDictionary;

        public PCSLibrary()
        {
            this.tuples = new List<Tuple<string, int, string>>();
            this.serverDictionary = new Dictionary<string, Process>();
            this.clientDictionary = new Dictionary<string, Process>();
        }

        public List<Tuple<string, int, string>> GetTuples()
        {
            return this.tuples;
        }

        public void AddTuple(Tuple<string, int, string> tuple)
        {
            this.tuples.Add(tuple);
        }

        public string[] GetWords()
        {
            return this.words;
        }

        public void SetWords(string[] words)
        {
            this.words = words;
        }

        public Dictionary<string, Process> GetServerDictionary()
        {
            return this.serverDictionary;
        }

        public void AddKeyValueToServerDictionary(string server_identifier, Process server_process)
        {
            this.serverDictionary.Add(server_identifier, server_process);
        }

        public Dictionary<string, Process> GetClientDictionary()
        {
            return this.clientDictionary;
        }

        public void AddKeyValueToClientDictionary(string client_identifier, Process client_process)
        {
            this.clientDictionary.Add(client_identifier, client_process);
        }
    }
}
