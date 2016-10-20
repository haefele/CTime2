using System;
using System.Collections.Generic;

namespace CTime2.Core.Services.EmployeeGroups.Data
{
    public class EmployeeGroup : IEquatable<EmployeeGroup>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> EmployeeIds { get; set; }

        #region Equality
        public bool Equals(EmployeeGroup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this.Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((EmployeeGroup) obj);
        }

        public override int GetHashCode()
        {
            return (this.Id != null ? this.Id.GetHashCode() : 0);
        }

        public static bool operator ==(EmployeeGroup left, EmployeeGroup right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EmployeeGroup left, EmployeeGroup right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}