using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Common
{
    public class UpdateStatus
    {
        [Required(ErrorMessage = "请选择菜单")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short Status { get; set; }
    }
}