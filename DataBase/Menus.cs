namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Menus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menus()
        {
            OrgMenu = new HashSet<OrgMenu>();
            RoleMenu = new HashSet<RoleMenu>();
        }

        public Guid Id { get; set; }

        public Guid PId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Controller { get; set; }

        [StringLength(1000)]
        public string Action { get; set; }

        public short Type { get; set; }

        [Required]
        [StringLength(2000)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Icon { get; set; }

        public short OrderNum { get; set; }

        public short Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrgMenu> OrgMenu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoleMenu> RoleMenu { get; set; }
    }
}
