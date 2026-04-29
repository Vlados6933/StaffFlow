using Microsoft.AspNetCore.Mvc.Filters;

namespace StaffFlow.Filters.ResultFilters
{
    public class PersonsListResultFilter(ILogger<PersonsListResultFilter> logger) : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger = logger;
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            await next();

            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
        }
    }
}
