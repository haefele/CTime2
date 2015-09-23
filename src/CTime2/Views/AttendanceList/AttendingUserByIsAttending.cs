using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using CTime2.Core.Data;

namespace CTime2.Views.AttendanceList
{
    public class AttendingUserByIsAttending
    {
        public bool IsAttending { get; }
        public BindableCollection<AttendingUser> Users { get; }

        private AttendingUserByIsAttending(BindableCollection<AttendingUser> users)
        {
            this.IsAttending = users.Select(f => f.IsAttending).FirstOrDefault();
            this.Users = users;
        }

        public static IEnumerable<AttendingUserByIsAttending> Create(IEnumerable<AttendingUser> users)
        {
            var result =
                from user in users
                orderby user.IsAttending descending
                group user by user.IsAttending into g
                select new AttendingUserByIsAttending(new BindableCollection<AttendingUser>(g));

            return result;
        }
    }
}