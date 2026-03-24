using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiTeste.Exceptions;

namespace PokeApiTeste.Application.Exceptions
{
    public class InvalidColorNameException : AppException
    {
        public InvalidColorNameException(string message = "Nome da cor inválido vindo da API ou do cliente.")
            : base(400, "Invalid External Data", message) { }

        public InvalidColorNameException(string message, Exception inner)
            : base(400, "Invalid External Data", message, inner) { }
    }
}