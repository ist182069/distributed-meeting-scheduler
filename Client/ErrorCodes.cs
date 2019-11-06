using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client
{
    class ErrorCodes
    {
        internal static String DUPLICATED_SLOT = "You've added the same slot twice. Aborting...";
        internal static String DUPLICATED_INVITEE = "You've added the same invitee twice. Aborting...";
        internal static String NONEXISTENT_SCRIPT = "The script name you inserted could not be found.";
        internal static String INVALID_COMMAND = "Error: you must insert a valid command.";
    }
}
