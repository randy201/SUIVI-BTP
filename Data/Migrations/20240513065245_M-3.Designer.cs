﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using temp_back.Connexion;

#nullable disable

namespace temp_back.Data.Migrations
{
    [DbContext(typeof(App_Db_Context))]
    [Migration("20240513065245_M-3")]
    partial class M3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("temp_back.Models.All_type", b =>
                {
                    b.Property<int>("Id_all_type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_all_type"));

                    b.Property<DateOnly>("DateOnly_value")
                        .HasColumnType("date");

                    b.Property<DateTimeOffset>("DateTime_value")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("Double_value")
                        .HasPrecision(30, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("File_path")
                        .HasColumnType("text");

                    b.Property<int>("Int_value")
                        .HasColumnType("integer");

                    b.Property<string>("String_value")
                        .HasColumnType("text");

                    b.Property<TimeOnly>("TimeOnly_value")
                        .HasColumnType("time without time zone");

                    b.HasKey("Id_all_type");

                    b.ToTable("all_types");
                });

            modelBuilder.Entity("temp_back.Models.Detail_maison", b =>
                {
                    b.Property<int>("Id_detail_maison")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_detail_maison"));

                    b.Property<double>("Quantite")
                        .HasColumnType("double precision");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.Property<int>("TravauxId_travaux")
                        .HasColumnType("integer");

                    b.HasKey("Id_detail_maison");

                    b.HasIndex("TravauxId_travaux");

                    b.ToTable("detail_maisons");
                });

            modelBuilder.Entity("temp_back.Models.Devis", b =>
                {
                    b.Property<int>("Id_devis")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_devis"));

                    b.Property<DateOnly>("Date_Fin")
                        .HasColumnType("date");

                    b.Property<DateOnly>("Date_debut")
                        .HasColumnType("date");

                    b.Property<DateOnly>("Date_devis")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("Heurre_debut")
                        .HasColumnType("time without time zone");

                    b.Property<TimeOnly>("Heurre_fin")
                        .HasColumnType("time without time zone");

                    b.Property<double>("Prix_total")
                        .HasColumnType("decimal(20,2)");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.Property<int>("Type_finitionId_type_finition")
                        .HasColumnType("integer");

                    b.Property<int>("Type_maisonId_maison")
                        .HasColumnType("integer");

                    b.Property<int>("UtilisateurId_utilisateur")
                        .HasColumnType("integer");

                    b.HasKey("Id_devis");

                    b.HasIndex("Type_finitionId_type_finition");

                    b.HasIndex("Type_maisonId_maison");

                    b.HasIndex("UtilisateurId_utilisateur");

                    b.ToTable("devis");
                });

            modelBuilder.Entity("temp_back.Models.Maison", b =>
                {
                    b.Property<int>("Id_maison")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_maison"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Durrer_totale")
                        .HasColumnType("double precision");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Prix_total")
                        .HasColumnType("double precision");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.HasKey("Id_maison");

                    b.ToTable("maisons");
                });

            modelBuilder.Entity("temp_back.Models.Profil", b =>
                {
                    b.Property<int>("Id_profil")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_profil"));

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.HasKey("Id_profil");

                    b.ToTable("profils");
                });

            modelBuilder.Entity("temp_back.Models.Travaux", b =>
                {
                    b.Property<int>("Id_travaux")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_travaux"));

                    b.Property<int>("Durree")
                        .HasColumnType("integer");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Prix_unitaire")
                        .HasColumnType("decimal(20,2)");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.Property<int>("UniteId_unite")
                        .HasColumnType("integer");

                    b.HasKey("Id_travaux");

                    b.HasIndex("UniteId_unite");

                    b.ToTable("travaux");
                });

            modelBuilder.Entity("temp_back.Models.Type_finition", b =>
                {
                    b.Property<int>("Id_type_finition")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_type_finition"));

                    b.Property<double>("Augmentation")
                        .HasPrecision(0)
                        .HasColumnType("double precision");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.HasKey("Id_type_finition");

                    b.ToTable("type_finitions");
                });

            modelBuilder.Entity("temp_back.Models.Unite", b =>
                {
                    b.Property<int>("Id_unite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_unite"));

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.HasKey("Id_unite");

                    b.ToTable("unites");
                });

            modelBuilder.Entity("temp_back.Models.Utilisateur", b =>
                {
                    b.Property<int>("Id_utilisateur")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id_utilisateur"));

                    b.Property<string>("Email")
                        .HasColumnType("text unique");

                    b.Property<string>("Mot_de_passe")
                        .HasColumnType("text");

                    b.Property<string>("Nom")
                        .HasColumnType("text");

                    b.Property<string>("Prenom")
                        .HasColumnType("text");

                    b.Property<int>("ProfilId_profil")
                        .HasColumnType("integer");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("text unique");

                    b.HasKey("Id_utilisateur");

                    b.HasIndex("ProfilId_profil");

                    b.ToTable("utilisateurs");
                });

            modelBuilder.Entity("temp_back.Models.Detail_maison", b =>
                {
                    b.HasOne("temp_back.Models.Travaux", "Travaux")
                        .WithMany()
                        .HasForeignKey("TravauxId_travaux")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Travaux");
                });

            modelBuilder.Entity("temp_back.Models.Devis", b =>
                {
                    b.HasOne("temp_back.Models.Type_finition", "Type_finition")
                        .WithMany()
                        .HasForeignKey("Type_finitionId_type_finition")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("temp_back.Models.Maison", "Type_maison")
                        .WithMany()
                        .HasForeignKey("Type_maisonId_maison")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("temp_back.Models.Utilisateur", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId_utilisateur")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type_finition");

                    b.Navigation("Type_maison");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("temp_back.Models.Travaux", b =>
                {
                    b.HasOne("temp_back.Models.Unite", "Unite")
                        .WithMany()
                        .HasForeignKey("UniteId_unite")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Unite");
                });

            modelBuilder.Entity("temp_back.Models.Utilisateur", b =>
                {
                    b.HasOne("temp_back.Models.Profil", "Profil")
                        .WithMany()
                        .HasForeignKey("ProfilId_profil")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Profil");
                });
#pragma warning restore 612, 618
        }
    }
}
