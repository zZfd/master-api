namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrgMenu")]
    public partial class OrgMenu
    {
        public int Id { get; set; }

        public Guid Org { get; set; }

        public Guid Menu { get; set; }

        public virtual Menu Menu1 { get; set; }

        public virtual Org Org1 { get; set; }
    }
}
