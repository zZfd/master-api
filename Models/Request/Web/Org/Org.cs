using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Web.Org
{
    public class Org
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请选择父级部门")]
        public Guid PId { get; set; }
        [Required(ErrorMessage = "请输入部门名称")]
        public string Name { get; set; }

       

        [Required(ErrorMessage = "请选择部门状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
        [Required(ErrorMessage = "请进行排序")]
        public short OrderNum { get; set; }
        //菜单
        //[Required(ErrorMessage = "请选择部门菜单")]
        //public List<Guid> Menus { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
    }
    public class OrgStatus
    {
        [Required(ErrorMessage = "请选择部门")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
    }
}