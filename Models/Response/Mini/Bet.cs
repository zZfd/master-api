using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini
{
    public class BetDetail
    {
        public Guid id { get; set; }

        public string match { get; set; }
        public string team { get; set; }

        public DateTime time { get; set; }
        public string platform { get; set; }
        public bool isSuccess { get; set; }

        public decimal money { get; set; }

        public double odds { get; set; }
        public decimal profit { get; set; }

        public string remarks { get; set; }

        public Guid attachment { get; set; }

        public string attachmentName { get; set; }
    }
}