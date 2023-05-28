using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.AspNetCore.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                string msg = context.Exception.Message;
                var exception = context.Exception;
                var result = new ObjectResult(new ErrorResultModel
                {
                    Code = context.Exception.GetType().Name,
                    Message = exception.Message,
                });
                result.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = result;

            }
            context.ExceptionHandled = true; //异常已处理了

            return Task.CompletedTask;
        }
    }
    public class ErrorResultModel
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public object Data { get; set; }
    }
}
