using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApprovaOSAuthService.Core
{
    [Produces("application/json")]
    [Route("api/ApprovaOSBase")]
    public class ApprovaOSAPIBaseController : Controller
    {
        protected string Authorization
        {
            get
            {
                return Request.Headers["Authorization"];
            }
        }

        protected string Tenant
        {
            get
            {
                return Request.Headers["Tenant"];
            }
        }

        protected string LogicalId
        {
            get
            {
                return Request.Headers["LogicalId"];
            }
        }

        protected string ClientID
        {
            get
            {
                return Request.Headers["ClientID"];
            }
        }

        protected string ClientSecret
        {
            get
            {
                return Request.Headers["ClientSecret"];
            }
        }

    }
}