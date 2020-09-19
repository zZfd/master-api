using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Web.Member
{
    public class Member
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请输入用户名")]

        public string Name { get; set; }
        [Required(ErrorMessage = "请输入手机号")]

        public long Phone { get; set; }
        [Required(ErrorMessage = "请输入密码")]

        public string Password { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        [Required(ErrorMessage = "请选择状态")]

        public short Status { get; set; }
        public Guid[] Orgs { get; set; }
        public Guid[] Roles { get; set; }

    }
    public class MemberStatus
    {
        [Required(ErrorMessage = "请选择用户")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        public short Status { get; set; }
    }
}