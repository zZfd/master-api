namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Orgs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orgs()
        {
            MemOrg = new HashSet<MemOrg>();
            OrgMenu = new HashSet<OrgMenu>();
            Roles = new HashSet<Roles>();
        }

        public Guid Id { get; set; }

        public Guid PId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Code { get; set; }

        [StringLength(2000)]
        public string Icon { get; set; }

        public short Status { get; set; }

        public short OrderNum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemOrg> MemOrg { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrgMenu> OrgMenu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Roles> Roles { get; set; }
    }
}
