using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using ToDoApp.Resources;

namespace ToDoApp.WebApi.Handlers
{
    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ArgumentException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, context.Exception.Message);
            }
            else
            {
                ReportError(context);

#if DEBUG
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception.Message);
#else
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ValidationMessages.Internal_Server_Error);
#endif
            }
        }

        private void ReportError(HttpActionExecutedContext actionExecutedContext)
        {
            //string controller = null;
            //string action = null;
            //string clientIP = null;
            //string xForwardedFor = null;
            //string xRequestOrigin = null;
            //Exception ex = actionExecutedContext.Exception;

            //try
            //{
            //    if (ex.GetType() == typeof(UnauthorizedAccessException))
            //        return;

            //    if (actionExecutedContext.ActionContext != null)
            //    {
            //        var actionContext = actionExecutedContext.ActionContext;
            //        action = actionContext.GetActionName();
            //        controller = actionContext.GetControllerName();
            //        clientIP = actionContext.GetClientIP();
            //        xForwardedFor = actionContext.GetXForwardedFor();
            //        xRequestOrigin = actionContext.GetXRequestOrigin();
            //    }
            //}
            //catch
            //{
            //    // ignored
            //}

            //Task.Factory.StartNew(() =>
            //{
            //    try
            //    {
            //        LoggingParameterManager.SetLoggingParameter("ClientIP", clientIP);
            //        LoggingParameterManager.SetLoggingParameter("XForwardedFor", xForwardedFor);
            //        LoggingParameterManager.SetLoggingParameter("Controller", controller);
            //        LoggingParameterManager.SetLoggingParameter("Action", action);
            //        LoggingParameterManager.SetLoggingParameter("XRequestOrigin", xRequestOrigin);
            //        CommonServiceLocator<ILogService>.GetService().Error(ex.Message, ex);
            //        LoggingParameterManager.ClearLoggingParameters();
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //});
        }
    }
}