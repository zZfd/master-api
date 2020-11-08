namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Collection")]
    public partial class Collection
    {
        public long Id { get; set; }

        public Guid Member { get; set; }

        public Guid Article { get; set; }

        public DateTime Time { get; set; }

        public short Status { get; set; }

        public virtual Article Article1 { get; set; }

        public virtual Member Member1 { get; set; }
    }
}
