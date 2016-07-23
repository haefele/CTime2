using CTime2.Core.Common;
using UwCore.Common;

namespace CTime2.Core.Data
{
    public class License
    {
        public string Name { get; }

        public License(string name)
        {
            Guard.NotNull(name, nameof(name));

            this.Name = name;
        }
    }
}