namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FT_Score
    {
        public int Id { get; set; }

        public Guid Match { get; set; }

        public Guid Scorer { get; set; }

        public Guid Assistant { get; set; }

        public TimeSpan Time { get; set; }

        public short Flag { get; set; }

        public virtual FT_Match FT_Match { get; set; }

        public virtual FT_Player FT_Player { get; set; }

        public virtual FT_Player FT_Player1 { get; set; }
    }
}
