using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Football
{
    public class Team
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请选择父级")]
        public Guid PId { get; set; }
        [Required(ErrorMessage = "请输入名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入英文名")]
        public string EName { get; set; }

        [Required(ErrorMessage = "请选择标识")]
        [Range(minimum: Models.Config.FTeamFlag.country, maximum: Models.Config.FTeamFlag.team)]

        public short Flag { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
        [Required(ErrorMessage = "请进行排序")]
        public short OrderNum { get; set; }
    }

    
}