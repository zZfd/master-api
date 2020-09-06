namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MemRole")]
    public partial class MemRole
    {
        public int Id { get; set; }

        public Guid Member { get; set; }

        public Guid Role { get; set; }

        public virtual Member Member1 { get; set; }

        public virtual Role Role1 { get; set; }
    }
}
