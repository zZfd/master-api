using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Web.Member
{
    public class Member
    {
    }
    public class OrgMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long Phone { get; set; }
        public string[] Roles { get; set; }
    }
}