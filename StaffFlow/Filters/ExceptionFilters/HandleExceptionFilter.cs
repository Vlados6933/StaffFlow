using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StaffFlow.Filters.ExceptionFilters
{
    public class HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment) : IAsyncExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger = logger;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError("Exception filter {FilterName}.{Method}\n {ExceptionType}\n{ExceptionMassage}", nameof(HandleExceptionFilter), nameof(OnExceptionAsync), context.Exception.GetType().ToString(), context.Exception.Message);

            if (_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Task.CompletedTask;
        }
    }
}
