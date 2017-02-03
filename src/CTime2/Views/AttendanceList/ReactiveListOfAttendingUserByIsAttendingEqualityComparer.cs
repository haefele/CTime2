using System.Collections.Generic;
using Newtonsoft.Json;
using ReactiveUI;

namespace CTime2.Views.AttendanceList
{
    public class ReactiveListOfAttendingUserByIsAttendingEqualityComparer : IEqualityComparer<ReactiveList<AttendingUserByIsAttending>>
    {
        public bool Equals(ReactiveList<AttendingUserByIsAttending> x, ReactiveList<AttendingUserByIsAttending> y)
        {
            //Abuse json to check
            string xJson = JsonConvert.SerializeObject(x);
            string yJson = JsonConvert.SerializeObject(y);

            return xJson == yJson;
        }

        public int GetHashCode(ReactiveList<AttendingUserByIsAttending> obj)
        {
            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }
}