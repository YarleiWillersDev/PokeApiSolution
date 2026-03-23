using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;

namespace PokeApiTeste.Handlers
{
    public class DefaultHandler : IExceptionHandler
    {
        private readonly ILogger<DefaultHandler> _logger;
        private readonly IHostEnvironment _env;

        public DefaultHandler(ILogger<DefaultHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            _logger.LogError(exception, "Erro genérico 500: {Message}", exception.Message);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = 500,
                title = "Internal Server Error",
                message = "Algo deu muito errado!",
                details = _env.IsDevelopment() ? exception.StackTrace ?? "" : "Contate suporte"
            };

            await context.Response.WriteAsJsonAsync(response, ct);
            return true;
        }
    }
}