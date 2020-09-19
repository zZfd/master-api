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

        public virtual DbSet<Attachments> Attachments { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<MemOrg> MemOrg { get; set; }
        public virtual DbSet<MemRole> MemRole { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<OrgMenu> OrgMenu { get; set; }
        public virtual DbSet<Orgs> Orgs { get; set; }
        public virtual DbSet<RoleMenu> RoleMenu { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachments>()
                .Property(e => e.FileName)
                .IsUnicode(false);

            modelBuilder.Entity<Attachments>()
                .Property(e => e.FileExt)
                .IsUnicode(false);

            modelBuilder.Entity<Attachments>()
                .Property(e => e.AttachmentType)
                .IsUnicode(false);

            modelBuilder.Entity<Attachments>()
                .Property(e => e.FileType)
                .IsUnicode(false);

            modelBuilder.Entity<Logs>()
                .Property(e => e.IP)
                .IsUnicode(false);

            modelBuilder.Entity<Logs>()
                .Property(e => e.UserAgent)
                .IsUnicode(false);

            modelBuilder.Entity<Members>()
                .Property(e => e.PasswordSalt)
                .IsUnicode(false);

            modelBuilder.Entity<Members>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Members>()
                .Property(e => e.Avatar)
                .IsUnicode(false);

            modelBuilder.Entity<Members>()
                .HasMany(e => e.Attachments)
                .WithRequired(e => e.Members)
                .HasForeignKey(e => e.UpAccount)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Members>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.Members)
                .HasForeignKey(e => e.Member)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Members>()
                .HasMany(e => e.MemOrg)
                .WithRequired(e => e.Members)
                .HasForeignKey(e => e.Member);

            modelBuilder.Entity<Members>()
                .HasMany(e => e.MemRole)
                .WithRequired(e => e.Members)
                .HasForeignKey(e => e.Member);

            modelBuilder.Entity<Menus>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Menus>()
                .HasMany(e => e.OrgMenu)
                .WithRequired(e => e.Menus)
                .HasForeignKey(e => e.Menu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Menus>()
                .HasMany(e => e.RoleMenu)
                .WithRequired(e => e.Menus)
                .HasForeignKey(e => e.Menu);

            modelBuilder.Entity<Orgs>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Orgs>()
                .Property(e => e.Icon)
                .IsUnicode(false);

            modelBuilder.Entity<Orgs>()
                .HasMany(e => e.MemOrg)
                .WithRequired(e => e.Orgs)
                .HasForeignKey(e => e.Org);

            modelBuilder.Entity<Orgs>()
                .HasMany(e => e.OrgMenu)
                .WithRequired(e => e.Orgs)
                .HasForeignKey(e => e.Org)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Orgs>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.Orgs)
                .HasForeignKey(e => e.Org)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.Icon)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.MemRole)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.Role);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.RoleMenu)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.Role);
        }
    }
}
