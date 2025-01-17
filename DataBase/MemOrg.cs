namespace DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MemOrg")]
    public partial class MemOrg
    {
        public int Id { get; set; }

        public Guid Member { get; set; }

        public Guid Org { get; set; }

        public virtual Members Members { get; set; }

        public virtual Orgs Orgs { get; set; }
    }
}
