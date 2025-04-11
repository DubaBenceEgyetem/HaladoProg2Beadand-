﻿// <auto-generated />
using HaladoProg2Beadandó.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HaladoProg2Beadandó.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250411170833_VirtualInit")]
    partial class VirtualInit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HaladoProg2Beadandó.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("HaladoProg2Beadandó.Models.VirtualWallet", b =>
                {
                    b.Property<int>("VirtualWalletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VirtualWalletId"));

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("VirtualWalletId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("VirtualWallets", (string)null);
                });

            modelBuilder.Entity("HaladoProg2Beadandó.Models.VirtualWallet", b =>
                {
                    b.HasOne("HaladoProg2Beadandó.Models.User", "User")
                        .WithOne("VirtualWallet")
                        .HasForeignKey("HaladoProg2Beadandó.Models.VirtualWallet", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HaladoProg2Beadandó.Models.User", b =>
                {
                    b.Navigation("VirtualWallet")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
