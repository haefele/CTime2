using System.Collections.Generic;
using DynamicData.Binding;
using Newtonsoft.Json;
using ReactiveUI;

namespace CTime2.Views.AttendanceList
{
    public class ReactiveListOfAttendingUserByIsAttendingEqualityComparer : IEqualityComparer<ObservableCollectionExtended<AttendingUserByIsAttending>>
    {
        public bool Equals(ObservableCollectionExtended<AttendingUserByIsAttending> x, ObservableCollectionExtended<AttendingUserByIsAttending> y)
        {
            //Abuse json to check
            string xJson = JsonConvert.SerializeObject(x);
            string yJson = JsonConvert.SerializeObject(y);

            return xJson == yJson;
        }

        public int GetHashCode(ObservableCollectionExtended<AttendingUserByIsAttending> obj)
        {
            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }
}