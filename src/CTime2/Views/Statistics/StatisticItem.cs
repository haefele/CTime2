namespace CTime2.Views.Statistics
{
    public class StatisticItem
    {
        public string Name { get; }

        public string Value { get; }

        public StatisticItem(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}