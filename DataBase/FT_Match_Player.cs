namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FT_Match_Player")]
    public partial class FT_Match_Player
    {
        public long Id { get; set; }

        public Guid Match { get; set; }

        public Guid Player { get; set; }

        public short Flag { get; set; }

        public virtual FT_Match FT_Match { get; set; }

        public virtual FT_Player FT_Player { get; set; }
    }
}
