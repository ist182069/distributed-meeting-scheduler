using MSDAD.PCS.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MSDAD.PCS.Commands
{
    class AddRoom : Command
    {
        string[] words;
        public AddRoom(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            int room_capacity;
            string location_name, room_name;

            location_name = words[1];
            room_capacity = Int32.Parse(words[2]);
            room_name = words[3];

            base.pcsLibrary.AddLocationXML(location_name, room_capacity, room_name);                       

            return null;
        }
    }
}
