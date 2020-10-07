using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class Match
    {
        public Guid Id { get; set; }
        public string HomeTeam { get; set; }
        public string GuestTeam { get; set; }
        public short HomeScore { get; set; }
        public short GuestScore { get; set; }
        public short Total { get; set; }

        public DateTime Time { get; set; }
    }

    public class MatchScore
    {
        public string Team { get; set; }
        public string Scorer { get; set; }
        public string Assistant { get; set; }
        public string Keeper { get; set; }
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