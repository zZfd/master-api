using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini
{
    public class Collection
    {

        public long id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public DateTime time { get; set; }
        public string match { get; set; }
        public bool isTrue { get; set; }
        public int preference { get; set; }
        public int collection { get; set; }
        public string cover { get; set; }
    }
}