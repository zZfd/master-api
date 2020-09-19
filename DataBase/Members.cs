namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Members
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Members()
        {
            Attachments = new HashSet<Attachments>();
            Logs = new HashSet<Logs>();
            MemOrg = new HashSet<MemOrg>();
            MemRole = new HashSet<MemRole>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public long Phone { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string PasswordSalt { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(100)]
        public string NickName { get; set; }

        public DateTime? LastFailTime { get; set; }

        public short? FailTimes { get; set; }

        [StringLength(500)]
        public string Avatar { get; set; }

        public short Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachments> Attachments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Logs> Logs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemOrg> MemOrg { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemRole> MemRole { get; set; }
    }
}
