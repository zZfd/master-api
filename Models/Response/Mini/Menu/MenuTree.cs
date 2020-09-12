using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini.Menu
{
    public class MenuTree
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public List<MenuTree> Children { get; set; }
    }
}