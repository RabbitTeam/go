using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text;

namespace Sample.AspNetCore.Server.Filter
{
    internal class SignatureFilterAttribute : ActionFilterAttribute
    {
        #region Overrides of ActionFilterAttribute

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var query = request.Query;

            if (!query.Any())
                return;
            var isCheck = query.TryGetValue("sign", out var signValues);

            if (isCheck)
            {
                var sign = string.Join("",
                    query.OrderBy(i => i.Key).Where(i => i.Key != "sign").Select(i => i.Value.ToString()));
                isCheck = Convert.ToBase64String(Encoding.UTF8.GetBytes(sign)) == signValues.ToString();
            }
            if (!isCheck)
                context.Result = new BadRequestObjectResult(new { code = 1000, message = "签名错误" });
        }

        #endregion Overrides of ActionFilterAttribute
    }
}