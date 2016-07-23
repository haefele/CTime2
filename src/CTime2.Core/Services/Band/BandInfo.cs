using UwCore.Common;

namespace CTime2.Core.Services.Band
{
    public class BandInfo
    {
        public string Name { get; }

        public BandInfo(string name)
        {
            Guard.NotNull(name, nameof(name));

            this.Name = name;
        }
    }
}