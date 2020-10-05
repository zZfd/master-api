using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Common
{
    public class IdEName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string EName { get; set; }
    }
}