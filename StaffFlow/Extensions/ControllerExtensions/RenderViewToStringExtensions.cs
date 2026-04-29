using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace StaffFlow.Extensions.ControllerExtensions
{
    public static class RenderViewToStringExtensions
    {
        public static async Task<string> RenderViewToStringAsync<TModel>(this Controller controller, string viewName, TModel model)
        {
            controller.ViewData.Model = model;

            using var writer = new StringWriter();
            var viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

            if (viewResult.View == null)
                throw new ArgumentNullException($"View {viewName} не найдена");

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}
