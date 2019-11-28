using MSDAD.PCS.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MSDAD.PCS.Commands
{
    class Server : Command
    {
        bool write;
        string[] words; 

        public Server(ref PCSLibrary pcsLibrary, bool write) : base(ref pcsLibrary)
        {
            this.write = write;
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string server_arguments, server_identifier, server_url, server_path;
            //nao percebo para que servem estes
            int tolerated_faults, min_delay_ms, max_delay_ms;
            Tuple<string, Process> urlProcessTuple;

            server_identifier = words[1];
            server_url = words[2];
            tolerated_faults = Int32.Parse(words[3]);
            min_delay_ms = Int32.Parse(words[4]);
            max_delay_ms = Int32.Parse(words[5]);
            // usar os restantes campos que nao percebo para que sao words[3], words[4], words[5]

            if (write)
            {
                this.WriteLocationsXML();
            }                

            server_path = PCSUtils.AssembleCurrentPath(SERVER) + "\\" + SERVER_EXE;

            Process server_process = new Process();
            server_process.StartInfo.FileName = server_path;
            // TODO passar um XML ou whatever para criar as localizacoes
            server_arguments = server_identifier + " " + server_url + " " + tolerated_faults + " " + min_delay_ms + " " + max_delay_ms;
            server_process.StartInfo.Arguments = server_arguments;
            server_process.Start();
            
            base.pcsLibrary.AddKeyValueToServerDictionary(server_identifier, new Tuple<string, Process>(server_url, server_process));

            return null;
        }

        private void WriteLocationsXML()
        {
            string file_name, file_path, server_locations_path;
            TextWriter textWriter;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocationXML));
            server_locations_path = PCSUtils.AssembleCurrentPath(SERVER_SCRIPTS);

            foreach (LocationXML locationXML in base.pcsLibrary.GetLocationDictionary().Values)
            {
                file_name = locationXML.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                file_name += ".msdad";
                file_path = server_locations_path + "\\" + file_name;
                textWriter = new StreamWriter(file_path);
                xmlSerializer.Serialize(textWriter, locationXML);
                textWriter.Close();
            }
        }
    }
}
