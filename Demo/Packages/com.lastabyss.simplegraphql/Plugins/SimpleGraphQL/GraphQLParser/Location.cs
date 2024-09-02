using System.Text.RegularExpressions;

namespace SimpleGraphQL.GraphQLParser
{
    public readonly struct Location
    {
        private static readonly Regex _lineRegex = new Regex("\r\n|[\n\r]", RegexOptions.ECMAScript);

        public Location(ISource source, int position)
        {
            Line = 1;
            Column = position + 1;

            var matches = _lineRegex.Matches(source.Body);
            foreach (Match match in matches)
            {
                if (match.Index >= position)
                    break;

                Line++;
                Column = position + 1 - (match.Index + matches[0].Length);
            }
        }

        public int Column { get; }

        public int Line { get; }
    }
}
