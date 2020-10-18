using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class Player
    {
        public Guid Id { get; set; }
        public short Age { get; set; }
        public string Name { get; set; }
        public string EName { get; set; }
        public Guid Team { get; set; }
        public Guid Country { get; set; }
        public short Status { get; set; }
        public short Flag { get; set; }
    }
}