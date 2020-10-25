namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FT_Bet2
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Match { get; set; }

        [Required]
        [StringLength(200)]
        public string Team { get; set; }

        [Required]
        [StringLength(200)]
        public string Remarks { get; set; }

        public Guid Attachment { get; set; }

        [Column(TypeName = "date")]
        public DateTime Time { get; set; }

        public decimal Money { get; set; }

        public decimal Profit { get; set; }

        public double Odds { get; set; }

        [Required]
        [StringLength(50)]
        public string Platform { get; set; }

        public short IsSuccess { get; set; }
        public virtual Attachments Attachments { get; set; }

    }
}
