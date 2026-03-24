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
            _logger.LogWarning("🔥 Handler chamado! Tipo: {Tipo}, IsAppException: {IsApp}",
                exception.GetType().Name, exception is AppException);

            if (exception is AppException appException)
            {
                _logger.LogWarning("✅ É AppException! Status: {Status}", appException.StatusCode);

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


            _logger.LogWarning("❌ NÃO é AppException");
            return false;
        }
    }
}