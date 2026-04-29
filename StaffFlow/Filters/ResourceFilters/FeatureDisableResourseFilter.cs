using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StaffFlow.Filters.ResourceFilters
{
    public class FeatureDisableResourseFilter(ILogger<FeatureDisableResourseFilter> logger, bool isDisabled = true) : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisableResourseFilter> _logger = logger;
        private readonly bool _isDisabled = isDisabled;
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(FeatureDisableResourseFilter), nameof(OnResourceExecutionAsync));

            if (_isDisabled)
            {
                //context.Result = new NotFoundResult(); // 404 - Not Found

                context.Result = new StatusCodeResult(StatusCodes.Status501NotImplemented); 
            }
            else
                await next();

            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(FeatureDisableResourseFilter), nameof(OnResourceExecutionAsync));

        }
    }
}
