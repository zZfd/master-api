namespace MiniDB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DB : DbContext
    {
        public DB()
            : base("name=MiniDB")
        {
        }

        public virtual DbSet<Member> Member { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .Property(e => e.OpenId)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.NickName)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Province)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Country)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.AvatarUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.UnionId)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.SessionKey)
                .IsUnicode(false);
        }
    }
}
