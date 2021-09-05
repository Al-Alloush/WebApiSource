using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.ApiErrorHandlers
{
    public class ApiErrorResponse
    {
        // every error is going to have these two properities
        public int StatusCode { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 202=NoContent, 400=BadRequest, 401=Unauthorized; 
        /// 403=NoPermissions, 404=NotFound, 405=MethodNotAllowed, 500=ServerError
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        public ApiErrorResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            // fancy switch expressions
            return statusCode switch
            {
                202 => "return null",
                400 => "you have made a bad request!",
                401 => "you are not Authorized",
                403 => "you don't have the permissions",
                404 => "Resource not found!",
                405 => "Method Not Allowed, A request was made of a resource using a request method not supported by that resource; for example, using GET on a form which requires data to be presented via POST, or using PUT on a read-only resource.",
                500 => "Server Error",
                _ => "Somthing Wrong!, please Call the Support"/* _ => the default in switch */
            };
        }
    }
}
