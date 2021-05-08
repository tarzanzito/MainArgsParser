using System;

namespace Candal.MainArgumentsParser.Base
{
    public sealed class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}
