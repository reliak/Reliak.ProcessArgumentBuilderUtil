using System.Text;

namespace Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers
{
    // see also: http://resources.mpi-inf.mpg.de/departments/rg1/teaching/unixffb-ss98/quoting-guide.html

    /// <summary>
    /// Escape handler for arguments that are targeted for Posix shell
    /// </summary>
    public class PosixShellArgumentEscapeHandler : ArgumentEscapeHandlerBase
    {
        public PosixShellArgumentEscapeHandler(string sigil = "-")
            : base(sigil)
        { }

        protected override char[] CharactersRequiringEscape => new char[]
        {
            '|', '&', ';', '<', '>', '(', ')', '$', '`', '\\', '"', '\'', ' ',
            '\t', '\r', '\n', '*', '?', '[', '#', '~', '=', '%'
        };

        public override string Escape(string argument)
        {
            if(!NeedsEscape(argument))
            {
                return argument;
            }

            var sb = new StringBuilder(argument);
            sb.Replace("\\\n", "").Replace("\\\r", ""); // remove escaped newlines (handle line continuations)

            if(sb.Length == 0)
            {
                return "\"\""; // empty string requires quoting
            }

            sb.Replace("\\", "\\\\")
              .Replace("$", "\\$")
              .Replace("`", "\\`")
              .Replace("\"", "\\\"");

            sb.Insert(0, '"').Append('"');

            return sb.ToString();
        }
    }
}