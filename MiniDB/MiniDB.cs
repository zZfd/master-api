namespace MiniDB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MiniDB : DbContext
    {
        public MiniDB()
            : base("name=MiniDB")
        {
        }

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Bet> Bet { get; set; }
        public virtual DbSet<Collection> Collection { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Order> Order { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .Property(e => e.Match)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .Property(e => e.Recommand)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .Property(e => e.Analysis)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .HasMany(e => e.Collection1)
                .WithRequired(e => e.Article1)
                .HasForeignKey(e => e.Article)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.Article1)
                .HasForeignKey(e => e.Article)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<Attachment>()
                .HasMany(e => e.Article)
                .WithOptional(e => e.Attachment1)
                .HasForeignKey(e => e.Attachment);

            modelBuilder.Entity<Attachment>()
                .HasMany(e => e.Bet)
                .WithOptional(e => e.Attachment1)
                .HasForeignKey(e => e.Attachment);

            modelBuilder.Entity<Bet>()
                .Property(e => e.Match)
                .IsUnicode(false);

            modelBuilder.Entity<Bet>()
                .Property(e => e.Team)
                .IsUnicode(false);

            modelBuilder.Entity<Bet>()
                .Property(e => e.Platform)
                .IsUnicode(false);

            modelBuilder.Entity<Bet>()
                .Property(e => e.Remarks)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.IP)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.UserAgent)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.OpenId)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.NickName)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Phone)
                .IsFixedLength();

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

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Article)
                .WithRequired(e => e.Member)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Bet)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Collection)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Log)
                .WithOptional(e => e.Member1)
                .HasForeignKey(e => e.Member);

            modelBuilder.Entity<Member>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.Member1)
                .HasForeignKey(e => e.Member)
                .WillCascadeOnDelete(false);
        }
    }
}
