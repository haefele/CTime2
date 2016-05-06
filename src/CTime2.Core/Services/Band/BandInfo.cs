using CTime2.Core.Common;
using JetBrains.Annotations;
using UwCore.Common;

namespace CTime2.Core.Services.Band
{
    public class BandInfo
    {
        [NotNull]
        public string Name { get; }

        public BandInfo([NotNull]string name)
        {
            Guard.NotNull(name, nameof(name));

            this.Name = name;
        }
    }
}