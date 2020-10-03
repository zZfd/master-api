using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Manage
{
    public class Role
    {
        public Guid Id { get; set; }
        public Guid PId { get; set; }
        public string Name { get; set; }

        public short OrderNum { get; set; }
    }
    public class RoleTree
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        public List<RoleTree> Children { get; set; }
    }
}