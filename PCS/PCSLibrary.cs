using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS
{
    class PCSLibrary
    {
        string[] words;

        List<Tuple<string, int, string>> tuples;
        Dictionary<string, string> serverDictionary;
        
        public PCSLibrary()
        {
            this.tuples = new List<Tuple<string, int, string>>();
            this.serverDictionary = new Dictionary<string, string>();
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

        public Dictionary<string, string> GetServerDictionary()
        {
            return this.serverDictionary;
        }

        public void AddKeyValueToServerDictionary(string server_identifier, string server_url)
        {
            this.serverDictionary.Add(server_identifier, server_url);
        }
    }
}
