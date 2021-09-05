using Infrastructure.DataApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.ApiErrorHandlers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]/*to not display https://localhost:5001/swagger/ as an error in api swagger*/

    public class BuggyController : ControllerBase
    {

        private readonly AppDbContext _context;
        public BuggyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiErrorResponse(400));
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            return NotFound(new ApiErrorResponse(404));
        }

        [HttpPost("MethodNotAllowed")]
        public ActionResult<string> PostMethodNotAllowed()
        {
            return "Http Method is Post, Not Allowed to Get";
        }

        [HttpGet("testAuth")]
        [Authorize]
        public ActionResult<string> GetSecretText()
        {
            return "secret stuff";
        }

        [HttpGet("serverError")]
        public ActionResult GetServerError()
        {
            // when get an exception
            var user = _context.Users.Find(42);
            var thingToReturn = user.ToString();
            return Ok(thingToReturn);
        }

        [HttpGet("throwException")]
        public ActionResult<string> throwException()
        {
            throw new Exception("throw Exception");
        }

        [HttpGet("validationError/{id}")]
        public ActionResult GetValidationError(int id)
        {
            // validation Error
            return ValidationProblem();
        }
    }
}
