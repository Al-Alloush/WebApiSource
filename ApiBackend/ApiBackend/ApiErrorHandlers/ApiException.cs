using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.ApiErrorHandlers
{
    public class ApiException : ApiErrorResponse
    {

        // because we don't have a parameter constructor in ApiResponse, then we have to provide a constructor inside the class that's deriving from it.
        public ApiException(int statusCode, string message = null, string details = null) : base(statusCode, message)
        {
            Details = details;
        }

        // to contain the stack trace that return in Server Error response
        public string Details { get; set; }
    }
}
