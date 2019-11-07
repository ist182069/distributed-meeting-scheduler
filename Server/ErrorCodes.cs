using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    class ErrorCodes
    {
        internal static String MEETING_ALREADY_CANCELED = "That meeting is already CANCELLED.";
        internal static String MEETING_ALREADY_SCHEDULED = "That meeting is already SCHEDULED";
        internal static String CLIENT_IS_ALREADY_CANDIDATE = "You're already signed to that meeting.";
        internal static String CLIENT_IS_NOT_INVITED = "You're not invited to that meeting.";
        internal static String INVALID_SLOT = "One or more slots does not match the proposed for that meeting.";
        internal static String NOT_COORDINATOR = "You're not the coordinator of that meeting.";
        internal static String NONEXISTENT_MEETING = "That meeting doesn't exist.";
        internal static String NOT_A_LOCATION = "One or more of your given locations isn't in the system.";
        internal static String NOT_AN_INVITEE = "One or more of your invitees isn't in the system.";
        internal static String INVALID_PORT_FORMAT = "Invalid port format.";
        internal static String USER_WITH_SAME_ID = "That identifier is already taken.";
    }

}
