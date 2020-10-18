using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Football
{
    public class Player
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "请输入中文名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入英文名")]
        public string EName { get; set; }

        [Required(ErrorMessage = "请输入年龄")]
        public short Age { get; set; }

        [Required(ErrorMessage = "请选择球场位置")]
        public short Flag { get; set; }

        [Required(ErrorMessage = "请选择球队")]
        public Guid Team { get; set; }

        [Required(ErrorMessage = "请选择国籍")]
        public Guid Country { get; set; }

        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short Status { get; set; }
    }
    //分页查找用
    public class ListPlayer
    {
        public string Name { get; set; }
        public string EName { get; set; }
        [Range(minimum: Models.Config.FPlayerFlag.keeper - 1, maximum: Models.Config.FPlayerFlag.back)]
        public short Flag { get; set; }

        [Range(minimum: -1, maximum: 50)]
        public short MinAge { get; set; }
        [Range(minimum: -1, maximum: 50)] 
        public short MaxAge { get; set; }

        public Guid? Team { get; set; }

        public Guid? Country { get; set; }

        [Required(ErrorMessage = "请选择当前页")]
        [Range(minimum: 1, maximum: 99999)]
        public int PageIndex { get; set; }

        [Required(ErrorMessage = "请选择页面大小")]
        [Range(minimum: 1, maximum: 1000)]
        public int PageSize { get; set; }
    }
}