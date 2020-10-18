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
        public Guid? HomeTeam { get; set; }
        public Guid? GuestTeam { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Guid? League { get; set; }

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

    /// <summary>
    /// 比赛进球详情
    /// </summary>
    public class MatchDetail
    {
        [Required(ErrorMessage = "请选择比赛")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "请填写进球表")]
        public List<Score> Scores { get; set; }
    }

    /// <summary>
    /// 单个进球
    /// </summary>
    public class Score
    {
        [Required(ErrorMessage = "请选择进球队员")]
        public Guid Scorer { get; set; }

        public Guid Assistant { get; set; }

        [Required(ErrorMessage = "请选择守门员")]
        public Guid Keeper { get; set; }

        [Required(ErrorMessage = "请选择进球时间")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "请选择进球方式")]
        public short Flag { get; set; }
    }
}