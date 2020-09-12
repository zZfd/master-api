﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.App_Start
{
    public static class CreatDB
    {
        /// <summary>
        /// 检查创建数据库
        /// </summary>
        public static void Creat()
        {
            using (DataBase.DB db = new DataBase.DB())
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                    InsertMenus(db);
                    InsertOrgs(db);
                    InsertRoles(db);
                    InsertMembers(db);
                    db.SaveChanges();
                }
            }
        }

        public static void InsertMenus(DataBase.DB db)
        {
            //添加菜单根节点
            DataBase.Menu root = new DataBase.Menu
            {
                Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
                PId = Guid.Parse("00000000-0000-0000-0001-000000000000"),
                Name = "Manage",
                Controller = "",
                Action = "",
                Type = "ROOT",
                Code = "g8J1",
                Icon = "fa-paper-plane-o",
                OpenNewPage = false,
                OrderNum = 1,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            db.Menu.Add(root);
        }

        public static void InsertOrgs(DataBase.DB db)
        {
            DataBase.Org org = new DataBase.Org
            {
                Id = new Guid("00000000-0000-0000-0001-000000000000"),
                PId = new Guid("00000000-0000-0000-0001-000000000000"),
                Name = "好悦猪",
                Code = "efft",
                Status = (short)Models.Setting.NormalStauts.正常,
                Icon = "",
                OrgMenu = new List<DataBase.OrgMenu>(),
                OrderNum = 1
            };
            org.OrgMenu.Add(new DataBase.OrgMenu { Org = org.Id, Menu = Guid.Parse("00000000-0000-0000-0001-000000000000") });
            db.Org.Add(org);
        }

        public static void InsertRoles(DataBase.DB db)
        {
            DataBase.Role manage = new DataBase.Role
            {
                Id = new Guid("00000000-0000-0000-0001-000000000000"),
                PId = new Guid("00000000-0000-0000-0001-000000000000"),
                Name = "超级管理员",
                Code = "efft",
                Status = (short)Models.Setting.NormalStauts.正常,
                Icon = "",
                RoleMenu = new List<DataBase.RoleMenu>(),
                Org = new Guid("00000000-0000-0000-0001-000000000000"),
                OrderNum = 1
            };
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("00000000-0000-0000-0001-000000000000") });
            db.Role.Add(manage);
        }

        public static void InsertMembers(DataBase.DB db)
        {
            DataBase.Member manage = new DataBase.Member
            {
                Id = new Guid("00000000-0000-0001-0000-000000000000"),
                Name = "admin",
                Phone = 15988800323,
                Password = "E612E72F933348A3B96858CC9F5C1D0A29E92B75",
                PasswordSalt = "rI75pLIL",
                Code = "rI75pL",
                NickName = "超级管理员",
                LastFailTime = DateTime.Now,
                FailTimes = 0,
                Status = (short)Models.Setting.NormalStauts.正常,
                Avatar = "",
                MemRole = new List<DataBase.MemRole>(),
                MemOrg = new List<DataBase.MemOrg>()
            };
            manage.MemRole.Add(new DataBase.MemRole { Member = manage.Id, Role = new Guid("00000000-0000-0000-0001-000000000000") });
            manage.MemOrg.Add(new DataBase.MemOrg { Member = manage.Id, Org = new Guid("00000000-0000-0000-0001-000000000000") });
            db.Member.Add(manage);
        }
    }
}