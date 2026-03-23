using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.Exceptions
{
    [Serializable]
    public abstract class AppException : Exception
    {

        public int StatusCode { get; protected init; }
        public string Title { get; protected init; } = string.Empty;

        protected AppException(int statusCode, string title, string message) : base(message)
        {
            StatusCode = statusCode;
            Title = title;
        }

        protected AppException(int statusCode, string title, string message, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
            Title = title;
        }

        public AppException() { }

        public AppException(string message) : base(message) { }
    }
}