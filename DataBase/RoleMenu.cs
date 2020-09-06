namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoleMenu")]
    public partial class RoleMenu
    {
        public int Id { get; set; }

        public Guid Menu { get; set; }

        public Guid Role { get; set; }

        public virtual Menu Menu1 { get; set; }

        public virtual Role Role1 { get; set; }
    }
}
