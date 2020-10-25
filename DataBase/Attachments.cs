namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Attachments
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FileName { get; set; }

        public long FileSize { get; set; }

        [Required]
        [StringLength(50)]
        public string FileExt { get; set; }

        [Required]
        [StringLength(50)]
        public string AttachmentType { get; set; }

        public DateTime UpTime { get; set; }

        public Guid UpAccount { get; set; }

        public Guid Belong { get; set; }

        public short Status { get; set; }

        [Required]
        [StringLength(100)]
        public string FileType { get; set; }

        public virtual Members Members { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Bet> FT_Bet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<FT_Bet2> FT_Bet2 { get; set; }

    }
}
