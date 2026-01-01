using System;
using System.Collections.Generic;
using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Data;

public partial class GlobalFestsContext : DbContext
{
    public GlobalFestsContext()
    {
    }

    public GlobalFestsContext(DbContextOptions<GlobalFestsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Performer> Performers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WishList> WishList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07E41561E9");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Events__3214EC075F72CDD2");

            
            entity.ToTable(tb => tb.HasTrigger("ValidateEventDates"));

            //entity.Property(e => e.Approved).HasDefaultValue(false);

            entity.Property(e => e.Status).HasDefaultValue(0);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Country).WithMany(p => p.Events)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__CountryI__4316F928");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__Organize__412EB0B6");

            entity.HasOne(d => d.Type).WithMany(p => p.Events)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__TypeId__4222D4EF");

            entity.HasMany(d => d.Genres).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventGenr__Genre__46E78A0C"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventGenr__Event__45F365D3"),
                    j =>
                    {
                        j.HasKey("EventId", "GenreId").HasName("PK__EventGen__897C9847BA6C2B2A");
                        j.ToTable("EventGenres");
                    });

            entity.HasMany(d => d.Performers).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventPerformer",
                    r => r.HasOne<Performer>().WithMany()
                        .HasForeignKey("PerformerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventPerf__Perfo__4AB81AF0"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventPerf__Event__49C3F6B7"),
                    j =>
                    {
                        j.HasKey("EventId", "PerformerId").HasName("PK__EventPer__5447B344FBDE135D");
                        j.ToTable("EventPerformers");
                    });
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventTyp__3214EC07C69597A1");
        });

       

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC072F6FDFBE");
        });

        modelBuilder.Entity<Performer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Performe__3214EC078D675723");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Country).WithMany(p => p.Performers).HasConstraintName("FK__Performer__Avata__37A5467C");

           /* entity.Property(e => e.Approved)
            //.IsRequired()
            //.HasDefaultValue(false);*/

            entity.Property(e => e.Status).HasDefaultValue(0);

            entity.HasOne(d => d.Creator)
             .WithMany() 
             .HasForeignKey(d => d.CreatedBy)
             .OnDelete(DeleteBehavior.SetNull) 
             .HasConstraintName("FK_Performers_CreatedBy");

            entity.HasMany(d => d.Genres).WithMany(p => p.Performers)
            .UsingEntity<Dictionary<string, object>>(
             "PerformerGenre", // name in EF
             r => r.HasOne<Genre>().WithMany()
                 .HasForeignKey("GenreId")
                 .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("FK__Performer__Genre__7B264821"),
             l => l.HasOne<Performer>().WithMany()
                 .HasForeignKey("PerformerId")
                 .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("FK__Performer__Perfo__7A3223E8"),
             j =>
             {
                 j.HasKey("PerformerId", "GenreId").HasName("PK__PerformerGen__UniqueID");
                 j.ToTable("PerformerGenres"); 
             });

        });

    
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07E5AA7F24");
        });

        modelBuilder.Entity<SchemaVersion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SchemaVe__3214EC078E289D6C");

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3214EC0794E1BE2B");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("CheckTicketApproval");
                    tb.HasTrigger("PreventOrganizerBuyTicketOnOwnEvent");
                    tb.HasTrigger("ReduceTicketAmount");
                    tb.HasTrigger("RestoreTicketAmount");
                });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__EventId__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__UserId__4F7CD00D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC076909C8C2");

            entity.ToTable(tb => tb.HasTrigger("AutoVerifyAdmins"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Verified).HasDefaultValue(false);

            entity.HasOne(d => d.Country).WithMany(p => p.Users).HasConstraintName("FK__Users__CountryId__33D4B598");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__34C8D9D1");

        });

        modelBuilder.Entity<WishList>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.EventId }).HasName("PK__WishList__001C80CD02092819");

            entity.HasOne(d => d.User).WithMany(p => p.WishList)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishList__UserId__534D60F1");

            entity.HasOne(d => d.Event).WithMany(p => p.WishList)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishList__EventI__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
