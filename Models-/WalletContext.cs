using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WalletSICAI.Models;

public partial class WalletContext : DbContext
{
    public WalletContext()
    {
    }

    public WalletContext(DbContextOptions<WalletContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrativo> Administrativos { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<HistorialRecargaEstudiante> HistorialRecargaEstudiantes { get; set; }

    public virtual DbSet<Institucione> Instituciones { get; set; }

    public virtual DbSet<Recarga> Recargas { get; set; }

    public virtual DbSet<VwHistorialRecarga> VwHistorialRecargas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrativo>(entity =>
        {
            entity.HasKey(e => e.AdministrativoId).HasName("PK__Administ__320E52920CDCF077");

            entity.HasIndex(e => e.AdministrativoCedula, "UQ__Administ__91C417C77C0FF3F9").IsUnique();

            entity.Property(e => e.AdministrativoId)
                .ValueGeneratedNever()
                .HasColumnName("AdministrativoID");
            entity.Property(e => e.AdministrativoApellido).HasMaxLength(100);
            entity.Property(e => e.AdministrativoCedula)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.AdministrativoEmail)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.AdministrativoNombre).HasMaxLength(100);
            entity.Property(e => e.AdministrativoNombreCompleto)
                .HasMaxLength(201)
                .HasComputedColumnSql("(([AdministrativoNombre]+' ')+[AdministrativoApellido])", false);
            entity.Property(e => e.AdministrativoPassword).HasMaxLength(50);
            entity.Property(e => e.AdministrativoSalt).HasMaxLength(32);
            entity.Property(e => e.AministrativoPuesto).HasMaxLength(150);
            entity.Property(e => e.InstitucionId).HasColumnName("InstitucionID");

            entity.HasOne(d => d.Institucion).WithMany(p => p.Administrativos)
                .HasForeignKey(d => d.InstitucionId)
                .HasConstraintName("FK_Administrativo_Instititucion");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.EstudianteId).HasName("PK__Estudian__6F76833800058BCC");

            entity.HasIndex(e => e.EstudianteCedula, "UQ__Estudian__E3012E1D4932A87F").IsUnique();

            entity.Property(e => e.EstudianteId)
                .ValueGeneratedNever()
                .HasColumnName("EstudianteID");
            entity.Property(e => e.EstudianteApellido).HasMaxLength(100);
            entity.Property(e => e.EstudianteCedula)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.EstudianteEmailInstitucion).HasMaxLength(150);
            entity.Property(e => e.EstudianteEmailPersonal).HasMaxLength(150);
            entity.Property(e => e.EstudianteNombre).HasMaxLength(100);
            entity.Property(e => e.EstudianteNombreCompleto)
                .HasMaxLength(201)
                .HasComputedColumnSql("(([EstudianteNombre]+' ')+[EstudianteApellido])", false);
            entity.Property(e => e.EstudianteNumeroTelefonico)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.EstudianteSeccion)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.InstitucionId).HasColumnName("InstitucionID");

            entity.HasOne(d => d.Institucion).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.InstitucionId)
                .HasConstraintName("FK_Estudiantes_Institucion");
        });

        modelBuilder.Entity<HistorialRecargaEstudiante>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__Historia__975206EF61545E3A");

            entity.ToTable("HistorialRecargaEstudiante");

            entity.Property(e => e.HistorialId).HasColumnName("HistorialID");
            entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");
            entity.Property(e => e.RecargaId).HasColumnName("RecargaID");

            entity.HasOne(d => d.Estudiante).WithMany(p => p.HistorialRecargaEstudiantes)
                .HasForeignKey(d => d.EstudianteId)
                .HasConstraintName("FK_Historial_Estudiante");

            entity.HasOne(d => d.Recarga).WithMany(p => p.HistorialRecargaEstudiantes)
                .HasForeignKey(d => d.RecargaId)
                .HasConstraintName("FK_Historial_Recarga");
        });

        modelBuilder.Entity<Institucione>(entity =>
        {
            entity.HasKey(e => e.InstitucionId).HasName("PK__Instituc__706D41E993323C46");

            entity.HasIndex(e => e.CedulaJuridica, "UQ__Instituc__024C516541E4417A").IsUnique();

            entity.Property(e => e.InstitucionId)
                .ValueGeneratedNever()
                .HasColumnName("InstitucionID");
            entity.Property(e => e.CedulaJuridica)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.InstitucionNombre).HasMaxLength(200);
        });

        modelBuilder.Entity<Recarga>(entity =>
        {
            entity.HasKey(e => e.RecargaId).HasName("PK__Recargas__018A5903E1D95803");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_AumentoMontoEstudiante");
                    tb.HasTrigger("trg_InsertarHistorialRecarga");
                });

            entity.Property(e => e.RecargaId).HasColumnName("RecargaID");
            entity.Property(e => e.AdministrativoId).HasColumnName("AdministrativoID");
            entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");
            entity.Property(e => e.ModoPagoRecarga)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SolicitanteRecargaApellido).HasMaxLength(100);
            entity.Property(e => e.SolicitanteRecargaCedula)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.SolicitanteRecargaEmail).HasMaxLength(150);
            entity.Property(e => e.SolicitanteRecargaNombre).HasMaxLength(100);
            entity.Property(e => e.SolicitanteRecargaNombreCompleto)
                .HasMaxLength(201)
                .HasComputedColumnSql("(([SolicitanteRecargaNombre]+' ')+[SolicitanteRecargaApellido])", false);

            entity.HasOne(d => d.Administrativo).WithMany(p => p.Recargas)
                .HasForeignKey(d => d.AdministrativoId)
                .HasConstraintName("FK_Recarga_Administrativo");

            entity.HasOne(d => d.Estudiante).WithMany(p => p.Recargas)
                .HasForeignKey(d => d.EstudianteId)
                .HasConstraintName("FK_Recarga_Estudiante");
        });

        modelBuilder.Entity<VwHistorialRecarga>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_HistorialRecargas");

            entity.Property(e => e.AdministrativoNombreCompleto).HasMaxLength(201);
            entity.Property(e => e.EstudianteCedula)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.EstudianteNombreCompleto).HasMaxLength(201);
            entity.Property(e => e.HistorialId).HasColumnName("HistorialID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
