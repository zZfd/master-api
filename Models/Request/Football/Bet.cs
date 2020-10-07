using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Football
{
    public class Bet
    {
        [Required(ErrorMessage = "请选择比赛")]
        public Guid Match { get; set; }

        [Required(ErrorMessage = "请选择球队")]
        public Guid Team { get; set; }

        [Required(ErrorMessage = "请输入投注信息")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "请选择投注时间")]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "请输入投注金额")]
        public decimal Money { get; set; }

        public decimal Profit { get; set; }

        [Required(ErrorMessage = "请输入当前赔率")]
        public decimal Odds { get; set; }

        [Required(ErrorMessage = "请输入投注平台")]
        public string Platform { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class ListBet
    {
        public Guid Team { get; set; }

        public Guid Match { get; set; }


        public decimal  MinMoney 
        {
            get { return MinMoney; }
            set { MinMoney = -1; }
        }

        public decimal MaxMoney
        {
            get { return MaxMoney; }
            set { MaxMoney = -1; }
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        public string Platform { get; set; }

        public bool IsSuccess { get; set; }
      

        [Required(ErrorMessage = "请选择当前页")]
        [Range(minimum: 1, maximum: 99999)]
        public int PageIndex { get; set; }

        [Required(ErrorMessage = "请选择页面大小")]
        [Range(minimum: 1, maximum: 1000)]
        public int PageSize { get; set; }
    }
}