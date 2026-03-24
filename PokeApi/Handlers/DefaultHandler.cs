using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "Algo deu muito errado!"
            };

            context.Response.StatusCode = problem.Status.Value;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}