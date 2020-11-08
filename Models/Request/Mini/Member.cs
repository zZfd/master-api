using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    public class Member
    {
    }
    public class SignUp
    {
        [Required]
        // 获取登录凭证 login
        public string code { get; set; }

        [Required]
        // 加密算法的初始向量  getUserInfo
        public string iv { get; set; }

        [Required]
        // 包括敏感数据在内的完整用户信息的加密数据  getUserInfo
        public string encryptedData { get; set; }
    }

   
}