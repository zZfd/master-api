namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Article")]
    public partial class Article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Article()
        {
            Collection1 = new HashSet<Collection>();
            Order = new HashSet<Order>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public Guid Author { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [StringLength(100)]
        public string Match { get; set; }

        [Required]
        [StringLength(100)]
        public string Recommand { get; set; }

        [Column(TypeName = "text")]
        public string Analysis { get; set; }

        public Guid? Attachment { get; set; }

        public bool? IsTrue { get; set; }

        public int Preference { get; set; }

        public int Collection { get; set; }

        public decimal Money { get; set; }

        public short Status { get; set; }

        public virtual Attachment Attachment1 { get; set; }

        public virtual Member Member { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Collection> Collection1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
