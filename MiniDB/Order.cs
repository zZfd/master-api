namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public Guid Id { get; set; }

        public Guid Member { get; set; }

        public Guid Article { get; set; }

        public decimal Money { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime TimeExpire { get; set; }

        public short Status { get; set; }

        public virtual Article Article1 { get; set; }

        public virtual Member Member1 { get; set; }
    }
}
