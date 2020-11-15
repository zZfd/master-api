namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WalletLog")]
    public partial class WalletLog
    {
        public long Id { get; set; }

        public Guid Member { get; set; }

        public Guid? Buyer { get; set; }

        public Guid? Article { get; set; }

        public decimal Money { get; set; }

        public DateTime Time { get; set; }

        public bool Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Remarks { get; set; }

        public virtual Article Article1 { get; set; }

        public virtual Member Member1 { get; set; }

        public virtual Member Member2 { get; set; }
    }
}
