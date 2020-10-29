namespace MiniDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Member")]
    public partial class Member
    {
        [Key]
        [StringLength(50)]
        public string OpenId { get; set; }

        [Required]
        [StringLength(50)]
        public string NickName { get; set; }

        public short Gender { get; set; }

        [Required]
        [StringLength(20)]
        public string City { get; set; }

        [Required]
        [StringLength(20)]
        public string Province { get; set; }

        [Required]
        [StringLength(20)]
        public string Country { get; set; }

        [Required]
        [StringLength(200)]
        public string AvatarUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string UnionId { get; set; }

        [Required]
        [StringLength(100)]
        public string SessionKey { get; set; }

        public DateTime Time { get; set; }
    }
}
