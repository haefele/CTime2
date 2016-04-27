using CTime2.Core.Common;
using JetBrains.Annotations;

namespace CTime2.Core.Data
{
    public class License
    {
        [NotNull]
        public string Name { get; }

        public License([NotNull]string name)
        {
            Guard.NotNull(name, nameof(name));

            this.Name = name;
        }
    }
}