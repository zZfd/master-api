namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FT_Bet
    {
        public int Id { get; set; }

        public Guid Match { get; set; }

        public Guid Team { get; set; }

        [Required]
        [StringLength(200)]
        public string Bet { get; set; }

        [Column(TypeName = "date")]
        public DateTime Time { get; set; }

        public short Money { get; set; }

        [Required]
        [StringLength(50)]
        public string Platform { get; set; }

        public bool IsSuccess { get; set; }

        public virtual FT_Match FT_Match { get; set; }

        public virtual FT_Team FT_Team { get; set; }
    }
}
