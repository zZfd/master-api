namespace DataBase
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DB : DbContext
    {
        public DB()
            : base("name=DB")
        {
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<MemOrg> MemOrg { get; set; }
        public virtual DbSet<MemRole> MemRole { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Org> Org { get; set; }
        public virtual DbSet<OrgMenu> OrgMenu { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleMenu> RoleMenu { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>()
                .Property(e => e.FileName)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.FileExt)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.AttachmentType)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.FileType)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.IP)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.UserAgent)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.PasswordSalt)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Avatar)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Attachment)
                .WithRequired(e => e.Member)
                .HasForeignKey(e => e.UpAccount)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Log)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.MemOrg)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.MemRole)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member);

            modelBuilder.Entity<Menu>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Menu>()
                .HasMany(e => e.OrgMenu)
                .WithRequired(e => e.Menu1)
                .HasForeignKey(e => e.Menu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Menu>()
                .HasMany(e => e.RoleMenu)
                .WithRequired(e => e.Menu1)
                .HasForeignKey(e => e.Menu);

            modelBuilder.Entity<Org>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Org>()
                .Property(e => e.Icon)
                .IsUnicode(false);

            modelBuilder.Entity<Org>()
                .HasMany(e => e.MemOrg)
                .WithRequired(e => e.Org1)
                .HasForeignKey(e => e.Org);

            modelBuilder.Entity<Org>()
                .HasMany(e => e.OrgMenu)
                .WithRequired(e => e.Org1)
                .HasForeignKey(e => e.Org)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Org>()
                .HasMany(e => e.Role)
                .WithRequired(e => e.Org1)
                .HasForeignKey(e => e.Org)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Icon)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.MemRole)
                .WithRequired(e => e.Role1)
                .HasForeignKey(e => e.Role);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.RoleMenu)
                .WithRequired(e => e.Role1)
                .HasForeignKey(e => e.Role);
        }
    }
}
