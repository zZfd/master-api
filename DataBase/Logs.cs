namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Logs
    {
        public int Id { get; set; }

        public Guid Member { get; set; }

        public Guid Belong { get; set; }

        public DateTime LogTime { get; set; }

        [Required]
        [StringLength(100)]
        public string LogType { get; set; }

        [Required]
        [StringLength(400)]
        public string Remarks { get; set; }

        [Required]
        [StringLength(200)]
        public string IP { get; set; }

        [Required]
        [StringLength(2000)]
        public string UserAgent { get; set; }

        public virtual Members Members { get; set; }
    }
}
