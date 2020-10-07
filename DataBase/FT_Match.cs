namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FT_Match
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FT_Match()
        {
            FT_Bet = new HashSet<FT_Bet>();
            FT_Score = new HashSet<FT_Score>();
        }

        public Guid Id { get; set; }

        public Guid HomeTeam { get; set; }

        public Guid GuestTeam { get; set; }

        public DateTime Time { get; set; }

        public short Total { get; set; }

        public short HomeScore { get; set; }

        public short GuestScore { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Bet> FT_Bet { get; set; }

        public virtual FT_Team FT_Team_Home { get; set; }

        public virtual FT_Team FT_Team_Guest { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Score> FT_Score { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FT_Match_Player> FT_Match_Player { get; set; }

        
    }
}
