using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini
{
    public class Order
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string match { get; set; }

        public string author { get; set; }
        public Guid article { get; set; }

        public DateTime time { get; set; }
        public bool? isTrue { get; set; }
    }
}