using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Web.Menu
{
    public class Menu
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请选择父级菜单")]
        public Guid PId { get; set; }
        [Required(ErrorMessage = "请输入菜单名称")]

        public string Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
        [Required(ErrorMessage = "请选择菜单类型")]

        public short Type { get; set; }
        public string Code { get; set; }

        public string Icon { get; set; }
        [Required(ErrorMessage = "请进行菜单排序")]

        public short OrderNum { get; set; }
        [Required(ErrorMessage = "请选择菜单状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
    }
    public class MenuStatus
    {
        [Required(ErrorMessage = "请选择菜单")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short Status { get; set; }
    }
}