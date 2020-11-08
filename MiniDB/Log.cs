namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log")]
    public partial class Log
    {
        public long Id { get; set; }

        public Guid? Member { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [StringLength(400)]
        public string Remarks { get; set; }

        [Required]
        [StringLength(200)]
        public string IP { get; set; }

        [Required]
        [StringLength(2000)]
        public string UserAgent { get; set; }

        public virtual Member Member1 { get; set; }
    }
}
