using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class BetDetail
    {
        public Guid Id { get; set; }

        public Match Match { get; set; }
        public Guid Team { get; set; }

        public DateTime Time { get; set; }
        public string Platform { get; set; }
        public short Status { get; set; }

        public decimal Money { get; set; }

        public double Odds { get; set; }
        public decimal Profit { get; set; }

        public string Remarks { get; set; }

        public Guid Attachment { get; set; }

        public string AttachmentName { get; set; }
    }
    public class Bet
    {
        public Guid Id { get; set; }
        public string Match { get; set; }
        public Match MatchInfo { get; set; }
        public Guid Team { get; set; }
        public string Platform { get; set; }
        public short Status { get; set; }
        public DateTime Time { get; set; }
       
    }
}