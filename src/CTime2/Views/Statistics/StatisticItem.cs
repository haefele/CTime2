using System;

namespace CTime2.Views.Statistics
{
    public class StatisticItem
    {
        public string Name { get; }

        public string Value { get; }

        public Action ShowDetails { get; }

        public StatisticItem(string name, string value, Action showDetails = null)
        {
            this.Name = name;
            this.Value = value;
            this.ShowDetails = showDetails;
        }
    }
}