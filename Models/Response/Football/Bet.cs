using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class BetDetail
    {
        public Guid Id { get; set; }

        public string Match { get; set; }
        public string Team { get; set; }

        public DateTime Time { get; set; }
        public string Platform { get; set; }
        public string Status { get; set; }

        public decimal Money { get; set; }

        public float Odds { get; set; }
        public decimal Profit { get; set; }

        public string Remarks { get; set; }

        public Guid Attachment { get; set; }
    }
}