namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bet")]
    public partial class Bet
    {
        public Guid Id { get; set; }

        public Guid Member { get; set; }

        [Required]
        [StringLength(200)]
        public string Match { get; set; }

        [StringLength(200)]
        public string Team { get; set; }

        public DateTime Time { get; set; }

        public decimal Money { get; set; }

        public decimal Profit { get; set; }

        public double Odds { get; set; }

        [Required]
        [StringLength(20)]
        public string Platform { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; }

        public bool? IsSuccess { get; set; }

        public virtual Member Member1 { get; set; }
    }
}
