using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using CTime2.Core.Data;

namespace CTime2.Views.AttendanceList
{
    public class AttendingUserByIsAttending
    {
        public AttendanceState AttendanceState { get; }
        public BindableCollection<AttendingUser> Users { get; }

        private AttendingUserByIsAttending(BindableCollection<AttendingUser> users)
        {
            this.AttendanceState = users.Select(f => f.AttendanceState).FirstOrDefault();
            this.Users = users;
        }

        public static IEnumerable<AttendingUserByIsAttending> Create(IEnumerable<AttendingUser> users)
        {
            var result =
                from user in users
                orderby user.AttendanceState.IsAttending descending, user.AttendanceState.Name ascending, user.FirstName ascending
                group user by new { user.AttendanceState.IsAttending, user.AttendanceState.Name } into g
                select new AttendingUserByIsAttending(new BindableCollection<AttendingUser>(g));

            return result;
        }
    }
}