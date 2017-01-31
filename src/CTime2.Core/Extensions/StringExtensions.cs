namespace CTime2.Core.Extensions
{
    public static class StringExtensions
    {
        public static string MakeFirstCharacterUpperCase(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
                return self;

            string firstCharacter = self.Substring(0, 1);
            string rest = self.Substring(1);

            return firstCharacter.ToUpper() + rest;
        }
    }
}