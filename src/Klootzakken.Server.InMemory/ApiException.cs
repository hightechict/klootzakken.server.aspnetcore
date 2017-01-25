using System;

namespace Klootzakken.Server.InMemory
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }

        public ApiException(Exception inner) : base(inner.Message, inner)
        {
        }
    }
}