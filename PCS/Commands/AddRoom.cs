using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Tuple<string, int, string> tuple;

            location_name = words[1];
            room_capacity = Int32.Parse(words[2]);
            room_name = words[3];
            
            tuple = new Tuple<string, int, string>(location_name, room_capacity, room_name);
            base.pcsLibrary.AddTuple(tuple);

            return null;
        }
    }
}
