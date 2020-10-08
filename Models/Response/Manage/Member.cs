using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Manage { 
    public class Member
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public short Status { get; set; }
        public long Phone { get; set; }
        public List<IdName> Roles { get; set; }
        public List<IdName> Orgs { get; set; }
    }
    public class IdName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}