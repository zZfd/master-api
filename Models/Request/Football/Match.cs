using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Football
{
    public class Match
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请选择主队")]
        public Guid HomeTeam { get; set; }
        [Required(ErrorMessage = "请选择客队")]
        public Guid GuestTeam { get; set; }
        [Required(ErrorMessage = "请输入比赛时间")]
        public DateTime Time { get; set; }
    }

    public class ListMatches
    {
        public Guid HomeTeam { get; set; }
        public Guid GuestTeam { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid League { get; set; }

        [Required(ErrorMessage = "请选择当前页")]
        [Range(minimum: 1, maximum: 99999)]
        public int PageIndex { get; set; }

        [Required(ErrorMessage = "请选择页面大小")]
        [Range(minimum: 1, maximum: 1000)]
        public int PageSize { get; set; }
    }

    public class MatchScore
    {
        public Guid Id { get; set; }
        [Range(minimum:0,maximum:50)]
        public short HomeScore { get; set; }
        [Range(minimum: 0, maximum: 50)]
        public short GuestScore { get; set; }
    }
}