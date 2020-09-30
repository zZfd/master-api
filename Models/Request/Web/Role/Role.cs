using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Web.Role
{
    public class Role
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请选择父级角色")]
        public Guid PId { get; set; }
        [Required(ErrorMessage = "请输入角色名称")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请选择角色状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
        [Required(ErrorMessage = "请进行排序")]
        public short OrderNum { get; set; }
        [Required(ErrorMessage = "请选择部门")]
        public Guid Org { get; set; }
        //菜单
        [Required(ErrorMessage = "请选择角色菜单")]
        public List<Guid> Menus { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
    }
    public class RoleStatus
    {
        [Required(ErrorMessage = "请选择角色")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
    }
}