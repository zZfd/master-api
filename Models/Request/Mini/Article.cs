using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    public class Article
    {
        // 修改用Id
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        // 标题
        public string title { get; set; }

        [Required]
        [StringLength(100)]
        // 比赛
        public string match { get; set; }

        [Required]
        [StringLength(100)]
        // 推荐
        public string recommend { get; set; }

        [Required]
        [Range(0.00,500.00)]
        // 金额 0.00代表免费
        public decimal money { get; set; }

        // 分析
        public string analysis { get; set; }

        // 审核
        public short status { get; set; }

    }
}