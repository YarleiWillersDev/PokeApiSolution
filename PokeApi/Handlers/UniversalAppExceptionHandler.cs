using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using PokeApiTeste.Exceptions;

namespace PokeApiTeste.Handlers
{
    public class UniversalAppExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<UniversalAppExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public UniversalAppExceptionHandler(ILogger<UniversalAppExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is AppException appException)
            {
                _logger.LogError(exception, "Erro tratado: {Message}", exception.Message);

                httpContext.Response.StatusCode = appException.StatusCode;
                httpContext.Response.ContentType = "application/json";

                string details = _env.IsDevelopment()
                    ? $"{exception.Message} | Stack: {exception.StackTrace}"
                    : "Erro interno do servidor";

                var response = new
                {
                    statusCode = appException.StatusCode,
                    title = appException.Title,
                    message = appException.Message,
                    details = details
                };

                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

                return true;
            }

            return false;
        }
    }
}