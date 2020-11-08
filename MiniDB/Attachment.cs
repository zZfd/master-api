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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Attachment()
        {
            Article = new HashSet<Article>();
            Bet = new HashSet<Bet>();
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Article> Article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bet> Bet { get; set; }
    }
}
