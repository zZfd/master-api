using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Filter
{
    public static class Power
    {
       /// <summary>
       /// 权限验证
       /// </summary>
       /// <param name="menuId"></param>
       /// <param name="memberId"></param>
       /// <param name="db"></param>
       /// <returns></returns>
        public static bool Menu(Guid menuId, Guid memberId,DataBase.DB db)
        {
            var m = db.Menu.Find(menuId);
            if (m == null) return false;

            var mem = db.Member.Find(memberId);
            if (mem == null) return false;

            List<Guid> memberMenusIds = (from menus in db.Menu
                                         orderby menus.OrderNum
                                         where (
                                             from rolemenus in db.RoleMenu
                                             where (
                                                 from role in db.Role
                                                 where (
                                                    from roles in db.MemRole
                                                    where roles.Member == memberId
                                                    select roles.Role
                                                 ).Contains(role.Id) && role.Status == 0
                                                 select role.Id
                                             ).Contains(rolemenus.Role)
                                             select rolemenus.Menu
                                         ).Contains(menus.Id) && menus.Status == 0
                                         select menus.Id).ToList();


            if (!memberMenusIds.Contains(menuId)) return false;

            return true;
        }
    }
}