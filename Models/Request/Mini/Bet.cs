using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    /// <summary>
    /// 添加
    /// </summary>
    public class Bet
    {

        [Required(ErrorMessage = "请选择比赛")]
        public string match { get; set; }

        [Required(ErrorMessage = "请选择球队")]
        public string team { get; set; }

        [Required(ErrorMessage = "请输入投注信息")]
        [StringLength(200, MinimumLength = 5)]
        public string remarks { get; set; }

        [Required(ErrorMessage = "请选择投注时间")]
        public DateTime time { get; set; }

        [Required(ErrorMessage = "请输入投注金额")]
        public decimal money { get; set; }

        public decimal profit { get; set; }

        [Required(ErrorMessage = "请输入当前赔率")]
        public double odds { get; set; }


        [Required(ErrorMessage = "请输入投注平台")]
        public string platform { get; set; }

        [Required(ErrorMessage = "请选择状态")]
        public bool isSuccess { get; set; }
    }

    /// <summary>
    /// 修改
    /// </summary>
    public class BetUpdate {
        [Required(ErrorMessage = "请选择投注")]
        public Guid id { get; set; }

        [Required(ErrorMessage = "请输入盈利")]
        public decimal profit { get; set; }

        [Required(ErrorMessage = "请选择状态")]
        public bool isSuccess { get; set; }

    }

    /// <summary>
    /// 筛选
    /// </summary>
    public class ListBet
    {
        public ListBet()
        {

            pageIndex = 1;
            pageSize = 10;
        }
        public string team { get; set; }

        public string match { get; set; }


        public decimal minMoney { get; set; }


        public decimal maxMoney { get; set; }


        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }


        public string platform { get; set; }

        public bool? isSuccess { get; set; }


        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }


}