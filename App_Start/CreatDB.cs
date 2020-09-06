using System;
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
                Id = Guid.Parse("D4406EB9-C2D7-4BFD-8510-DCB417FA7F33"),
                PId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
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
            DataBase.Menu system = new DataBase.Menu
            {
                Id = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080"),
                PId = Guid.Parse("D4406EB9-C2D7-4BFD-8510-DCB417FA7F33"),
                Name = "系统设置",
                Controller = "",
                Action = "",
                Type = "MENU-1",
                Code = "g8Jl-vJu2",
                Icon = "fa-gears",
                OpenNewPage = false,
                OrderNum = 2,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            DataBase.Menu member = new DataBase.Menu
            {
                Id = Guid.Parse("3A6D1151-9580-4B4B-BE56-81BFCD39527B"),
                PId = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080"),
                Name = "用户管理",
                Controller = "Sys",
                Action = "Member",
                Type = "PAGE",
                Code = "g8Jl-vJu2-Pmr8",
                Icon = "fa-user",
                OpenNewPage = false,
                OrderNum = 2,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            DataBase.Menu role = new DataBase.Menu
            {
                Id = Guid.Parse("EF6C7C8B-0735-4F0C-B327-A3B6AFCFFF4D"),
                PId = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080"),
                Name = "角色管理",
                Controller = "Sys",
                Action = "Role",
                Type = "PAGE",
                Code = "g8Jl-vJu2-oiN2",
                Icon = "fa-group",
                OpenNewPage = false,
                OrderNum = 1,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            DataBase.Menu menu = new DataBase.Menu
            {
                Id = Guid.Parse("CCB94B3B-0CFD-4168-9621-63BFF56A057F"),
                PId = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080"),
                Name = "菜单管理",
                Controller = "Sys",
                Action = "Menu",
                Type = "PAGE",
                Code = "g8Jl-vJu2-0M8n",
                Icon = "fa-th-list",
                OpenNewPage = false,
                OrderNum = 3,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            DataBase.Menu organization = new DataBase.Menu
            {
                Id = Guid.Parse("315B3E00-C982-42EA-9AC8-866634BD97E9"),
                PId = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080"),
                Name = "组织管理",
                Controller = "Sys",
                Action = "Org",
                Type = "PAGE",
                Code = "g8Jl-vJu2-GlVc",
                Icon = "fa-th-list",
                OpenNewPage = false,
                OrderNum = 4,
                Status = (short)Models.Setting.NormalStauts.正常,
            };
            db.Menu.Add(root);
            db.Menu.Add(system);
            db.Menu.Add(member);
            db.Menu.Add(role);
            db.Menu.Add(menu);
            db.Menu.Add(organization);
            
        }

        public static void InsertRoles(DataBase.DB db)
        {
            DataBase.Role manage = new DataBase.Role
            {
                Id = new Guid("00000000-0000-0000-0001-000000000000"),
                Name = "超级管理员",
                Status = (short)Models.Setting.NormalStauts.正常,
                Icon = "",
                RoleMenu = new List<DataBase.RoleMenu>()
            };
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("D4406EB9-C2D7-4BFD-8510-DCB417FA7F33") });
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("EB90672C-D535-4994-ABFF-6963F2666080") });
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("3A6D1151-9580-4B4B-BE56-81BFCD39527B") });
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("EF6C7C8B-0735-4F0C-B327-A3B6AFCFFF4D") });
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("CCB94B3B-0CFD-4168-9621-63BFF56A057F") });
            manage.RoleMenu.Add(new DataBase.RoleMenu { Role = manage.Id, Menu = Guid.Parse("315B3E00-C982-42EA-9AC8-866634BD97E9") });
            db.Role.Add(manage);
        }

        public static void InsertMembers(DataBase.DB db)
        {
            DataBase.Member manage = new DataBase.Member
            {
                Id = new Guid("00000000-0001-0001-0000-000000000000"),
                Name = "admin",
                Password = "E612E72F933348A3B96858CC9F5C1D0A29E92B75",
                PasswordSalt = "rI75pLIL",
                Code = "rI75pL",
                NickName = "超级管理员",
                LastFailTime = DateTime.Now,
                FailTimes = 0,
                Status = (short)Models.Setting.NormalStauts.正常,
                Avatar = "",
                MemRole = new List<DataBase.MemRole>()
            };
            manage.MemRole.Add(new DataBase.MemRole { Member = manage.Id, Role = new Guid("00000000-0000-0000-0001-000000000000") });
            db.Member.Add(manage);
        }
    }
}