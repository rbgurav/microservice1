using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApprovaOSAuthService.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApprovaOSAuthService.Controllers
{
    [Route("api/ApprovaOSAuthService")]
    public class TokenController : ApprovaOSAPIBaseController
    {
        /// <summary>
        /// Approva OS Authorization Token validation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateToken")]
        public IActionResult ValidateToken()
        {
            TokenHandler tokenHandler = new TokenHandler();

            // Validate access token
            if (!tokenHandler.ValidateTokenAsync(Authorization))
            {
                return new UnauthorizedResult();
            }            

            // Return success result
            return new OkObjectResult(new { });
        }

        /// <summary>
        /// Approva OS Authorization Permission validation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidatePermission")]
        public IActionResult ValidatePermission()
        {
            // Return success result
            return Ok();
        }
    }
}
