using Microsoft.AspNetCore.Mvc.Filters;

namespace StaffFlow.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute(string key, string value, int order) : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly string _key = key;
        private readonly string _value = value;
        private readonly int _order = order;


        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = _key;
            filter.Value = _value;
            filter.Order = _order;

            return filter;
        }
    }


    public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger) : IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger = logger;
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; } 

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Before logic - ResponseHeaderActionFilter"); 

            await next();

            _logger.LogInformation("After logic - ResponseHeaderActionFilter");

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
