using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Manage
{
    public class Login
    {
        [Required(ErrorMessage ="请输入用户名")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }
    }
    public class Member
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请输入用户名")]

        public string Name { get; set; }
        [Required(ErrorMessage = "请输入手机号")]

        public long Phone { get; set; }
        //[Required(ErrorMessage = "请输入密码")]

        public string Password { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short Status { get; set; }
        [Required(ErrorMessage = "请选择部门")]
        public Guid[] Orgs { get; set; }
        [Required(ErrorMessage = "请选择角色")]
        public Guid[] Roles { get; set; }
        

    }
    //分页查找用
    public class ListMember
    {
        public string Name { get; set; }
        public long? Phone { get; set; }
        public Guid? Org { get; set; }
        public Guid? Role { get; set; }
        //public short Status { get; set; }
        [Required(ErrorMessage = "请选择当前页")]
        [Range(minimum:1,maximum:99999)]
        public int PageIndex { get; set; }
        [Required(ErrorMessage = "请选择页面大小")]
        [Range(minimum: 1,maximum:1000)]
        public int PageSize { get; set; }
    }
    public class MemberStatus
    {
        [Required(ErrorMessage = "请选择用户")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]

        public short Status { get; set; }
    }
}