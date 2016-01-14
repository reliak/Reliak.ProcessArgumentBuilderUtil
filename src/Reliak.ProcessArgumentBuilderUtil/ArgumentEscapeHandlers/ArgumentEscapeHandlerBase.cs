using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers
{
    public abstract class ArgumentEscapeHandlerBase : IArgumentEscapeHandler
    {
        public string Sigil { get; private set; }
        protected abstract char[] CharactersRequiringEscape { get; }

        protected ArgumentEscapeHandlerBase(string sigil)
        {
            Sigil = sigil;
        }

        public abstract string Escape(string argument);

        protected virtual bool NeedsEscape(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument) || argument.StartsWith(Sigil))
            {
                return true;
            }

            return argument.Any(c => CharactersRequiringEscape.Contains(c));
        }
    }
}
