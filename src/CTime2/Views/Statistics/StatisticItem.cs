using System;

namespace CTime2.Views.Statistics
{
    public class StatisticItem
    {
        public string Name { get; }

        public string SubTitle { get; }

        public string Value { get; }
        
        public Action ShowDetails { get; }

        public StatisticItem(string name, string subTitle, string value, Action showDetails = null)
        {
            this.Name = name;
            this.SubTitle = subTitle;
            this.Value = value;
            this.ShowDetails = showDetails;
        }

        public override string ToString()
        {
            string subTitle = string.IsNullOrWhiteSpace(this.SubTitle)
                ? string.Empty
                : $" ({this.SubTitle})";
            
            return $"{this.Name}{subTitle}:{Environment.NewLine}" +
                   $"{this.Value}";
        }
    }
}