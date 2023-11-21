﻿// <auto-generated />
using System;
using ACorp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ACorp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231121090307_AddRequestDataTable")]
    partial class AddRequestDataTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ACorp.Models.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Cipher")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<TimeSpan>("DecryptDuration")
                        .HasColumnType("time(6)");

                    b.Property<TimeSpan>("EncryptDuration")
                        .HasColumnType("time(6)");

                    b.Property<string>("FileExtension")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("ACorp.Models.RequestData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("RequestedId")
                        .HasColumnType("int");

                    b.Property<int>("RequesterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RequestData");
                });

            modelBuilder.Entity("ACorp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AesNik")
                        .HasColumnType("longtext");

                    b.Property<string>("AesPassword")
                        .HasColumnType("longtext");

                    b.Property<string>("AesPhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("DesNik")
                        .HasColumnType("longtext");

                    b.Property<string>("DesPassword")
                        .HasColumnType("longtext");

                    b.Property<string>("DesPhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Rc4Nik")
                        .HasColumnType("longtext");

                    b.Property<string>("Rc4Password")
                        .HasColumnType("longtext");

                    b.Property<string>("Rc4PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ACorp.Models.Document", b =>
                {
                    b.HasOne("ACorp.Models.User", null)
                        .WithMany("Documents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ACorp.Models.User", b =>
                {
                    b.Navigation("Documents");
                });
#pragma warning restore 612, 618
        }
    }
}
