using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid HomeTeam { get; set; }
        public Guid GuestTeam { get; set; }
        public short HomeScore { get; set; }
        public short GuestScore { get; set; }
        public short Total { get; set; }

        public DateTime Time { get; set; }
    }

    public class MatchScore
    {
        public Guid Team { get; set; }
        public Guid Scorer { get; set; }
        public Guid Assistant { get; set; }
        public Guid Keeper { get; set; }
        public TimeSpan Time { get; set; }
        public short Flag { get; set; }
    }
    public class MatchDetail
    {
        public Match Match { get; set; }

        public List<MatchScore> HomeScores { get; set; }

        public List<MatchScore> GuestScores { get; set; }
    }
}