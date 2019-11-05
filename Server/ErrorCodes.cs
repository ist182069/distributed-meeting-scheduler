using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    class ErrorCodes
    {
        internal static String MEETING_ALREADY_CANCELED = "That meeting is already CANCELLED.";
        internal static String MEETING_ALREADY_SCHEDULED = "That meeting is already SCHEDULED";
        internal static String CLIENT_IS_ALREADY_CANDIDATE = "You're already signed to that meeting.";
        internal static String CLIENT_IS_NOT_INVITED = "You're not invited to that meeting.";
        internal static String ONE_INVALID_SLOT = "One or more slots does not match the proposed for that meeting. However, you're a candidate for the valid ones.";
        internal static String ALL_INVALID_SLOTS = "None of your slots match the proposed for that meeting.";
        internal static String NOT_COORDINATOR = "You're not the coordinator of that meeting.";
        internal static String NONEXISTENT_MEETING = "That meeting doesn't exist.";
        internal static String NOT_A_LOCATION = "One or more of your given locations isn't valid.";
    }

}
