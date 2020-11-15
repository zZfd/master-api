namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Attachment")]
    public partial class Attachment
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; }

        public long FileSize { get; set; }

        [Required]
        [StringLength(50)]
        public string FileExt { get; set; }

        [Required]
        [StringLength(50)]
        public string AttachmentType { get; set; }

        [Required]
        [StringLength(100)]
        public string FileType { get; set; }

        public DateTime UpTime { get; set; }

        public Guid UpAccount { get; set; }

        public Guid Belong { get; set; }

        public short Status { get; set; }
    }
}
