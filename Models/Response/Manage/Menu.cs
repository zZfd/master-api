﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Manage
{
    public class Menu
    {
        public Guid Id { get; set; }
        public Guid PId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }

        public string Action { get; set; }
        public string Icon { get; set; }
        public short Type { get; set; }
        public short Status { get; set; }

        public int OrderNum { get; set; }
        public bool HasChildren { get; set; }
        //总共有多少部门具有此权限
        //public int OrgCount { get; set; }
        //总共有多少角色具有此权限
        //public int RoleCount { get; set; }
        //总共有多少用户具有此权限
        //public int MemberCount { get; set; }
    }

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