using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    public class Log
    {
        public Guid Member { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
    }
}