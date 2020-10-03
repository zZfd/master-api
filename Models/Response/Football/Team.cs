﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Football
{
    public class Team
    {
        public Guid Id { get; set; }
        public Guid PId { get; set; }
        public string Name { get; set; }

        public short OrderNum { get; set; }
    }
    public class FootballTree
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        public List<FootballTree> Children { get; set; }
    }
}