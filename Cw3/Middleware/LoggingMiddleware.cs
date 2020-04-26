using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";

                using(var reader=new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
                string file = "requestsLog.txt";
                string textLog = 
                    "[LOG]" + Environment.NewLine +
                    "   [PATH]" + Environment.NewLine + "   " +path + Environment.NewLine +
                    "   [METHOD]" + Environment.NewLine + "   " + method + Environment.NewLine +
                    "   [QUERY]" + Environment.NewLine + "   " + queryString + Environment.NewLine +
                    "   [BODY]" + Environment.NewLine + "   " + bodyStr + Environment.NewLine +
                    "[END]" + Environment.NewLine
                    ;

                if (!File.Exists(file))
                {
                    File.WriteAllText(file, textLog);
                }
                else
                {
                    File.AppendAllText(file, textLog);
                }
            }
            if(_next!=null)await _next(context);
        }
    }
}
