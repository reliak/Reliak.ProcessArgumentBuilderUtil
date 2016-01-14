using System.Text;
using System.Linq;

namespace Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers
{
    // see also:
    // http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    // https://msdn.microsoft.com/en-us/library/17w5ykft.aspx

    /// <summary>
    /// Escape handler for arguments that are targeted for Windows shell
    /// </summary>
    public class WindowsArgumentEscapeHandler : ArgumentEscapeHandlerBase
    {
        private static readonly char[] _metaCharacters = new char[]
        {
            '|', '&', '(', ')', '<', '>', '"',  '^', '%', '!'
        };

        public WindowsArgumentEscapeHandler(string sigil = "/")
            : base(sigil)
        {
            var whiteSpace = new char[] { ' ', '\t' };
            CharactersRequiringEscape = _metaCharacters.Concat(whiteSpace).ToArray();
        }

        protected override char[] CharactersRequiringEscape { get; }
        
        public override string Escape(string argument)
        {
            if (!NeedsEscape(argument))
            {
                return argument;
            }

            if(string.IsNullOrWhiteSpace(argument))
            {
                return $"\"{argument}\"";
            }

            var sb = new StringBuilder(argument);

            sb.Replace("\\\"", "\\\\\"")
              .Replace("\"", "\\\"");

            if (sb[sb.Length - 1] == '\\')
            {
                sb.Append('\\');
            }

            sb.Insert(0, '"').Append('"');

            if (argument.Any(c => _metaCharacters.Contains(c)))
            {
                for (int i = 0; i < sb.Length; ++i)
                {
                    if (_metaCharacters.Contains(sb[i]))
                    {
                        sb.Insert(i++, "^");
                    }
                }
            }

            return sb.ToString();
        }
    }
}