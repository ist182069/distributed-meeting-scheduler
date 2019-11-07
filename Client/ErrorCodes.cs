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
        internal static String INVALID_MIN_ATTENDES = "Mininum Attendes input showd be a positive integer (>1).";
        internal static String INVALID_N_SLOTS = "Number of slots input should be an positive integer (>1).";
        internal static String INVALID_N_INVITEES = "Number of invitees input should be an positive integer (>0).";
        internal static String INVALID_PORT_FORMAT = "Invalid port format.";
    }
}
