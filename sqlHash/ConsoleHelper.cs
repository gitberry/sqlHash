namespace sqlHash
{
    class ConsoleHelper
    {
        public static string deriveParameter(string[] args, string givenPrefix, string defaultValue)
        {
            var result = defaultValue;
            for (int n = 0; n < args.Length; n++)
            {
                if (args[n].Substring(0, 2) == givenPrefix)  // note: case sensitive
                {
                    result = args[n].Substring(givenPrefix.Length, args[n].Length - givenPrefix.Length);
                    break;
                }
            }
            return result;
        }
    }
}
