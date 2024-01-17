using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Attendance.Providers
{
    public class HandlerFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            var handler = context.HttpContext.Request.Query["handler"];
            var targetMethodName = context.HandlerMethod?.Name?.ToLower();
            if (handler.Count > 0 && (targetMethodName == null || targetMethodName == "onget"))
            {
                context.HttpContext.Response.StatusCode = 404;
            }
            await Task.CompletedTask;
        }

        async Task IAsyncPageFilter.OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            await next.Invoke();
        }
    }
}
