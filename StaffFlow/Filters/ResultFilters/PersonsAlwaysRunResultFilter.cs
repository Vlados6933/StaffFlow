using Microsoft.AspNetCore.Mvc.Filters;

namespace StaffFlow.Filters.ResultFilters
{
    public class PersonsAlwaysRunResultFilter : IAsyncAlwaysRunResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            await next();

            if (context.Filters.OfType<SkipFilter>().Any())
                return;
        }
    }
}
