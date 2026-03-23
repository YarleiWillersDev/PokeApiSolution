using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiTeste.Exceptions;

namespace PokeApiTeste.Application.Exceptions
{
    public class PokeApiException : AppException
    {
        public PokeApiException(string message = "PokeApi não está retornando as informações.")
            : base(503, "Connection Failed", message) { }
        
        public PokeApiException(string message, Exception inner)
            : base(503, "Connection Failed", message, inner) { }
    }
}