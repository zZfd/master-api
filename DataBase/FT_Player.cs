namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FT_Player
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FT_Player()
        {
            FT_Score = new HashSet<FT_Score>();
            FT_Score1 = new HashSet<FT_Score>();
            FT_Score2 = new HashSet<FT_Score>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string EName { get; set; }

        public short Age { get; set; }

        public short Flag { get; set; }

        public Guid Team { get; set; }

        public Guid Country { get; set; }

        public short Status { get; set; }

        public virtual FT_Team FT_Team { get; set; }

        public virtual FT_Team FT_Team_Country { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Score> FT_Score { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Score> FT_Score1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Score> FT_Score2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Match_Player> FT_Match_Player { get; set; }
    }
}
