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

        public virtual Menus Menus { get; set; }

        public virtual Roles Roles { get; set; }
    }
}
