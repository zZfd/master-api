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
        public Guid Id { get; set; }

        [Required(ErrorMessage = "请选择比赛")]
        public string Match { get; set; }

        [Required(ErrorMessage = "请选择球队")]
        public string Team { get; set; }

        [Required(ErrorMessage = "请输入投注信息")]
        [StringLength(200, MinimumLength = 5)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "请选择投注时间")]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "请输入投注金额")]
        public decimal Money { get; set; }

        public decimal Profit { get; set; }

        [Required(ErrorMessage = "请输入当前赔率")]
        public double Odds { get; set; }


        [Required(ErrorMessage = "请输入投注平台")]
        public string Platform { get; set; }

        [Required(ErrorMessage = "请选择状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short IsSuccess { get; set; }
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


        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }


        public string platform { get; set; }

        public short isSuccess { get; set; }


        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }


}