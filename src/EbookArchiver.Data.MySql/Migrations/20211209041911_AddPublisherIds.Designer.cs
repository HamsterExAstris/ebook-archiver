﻿// <auto-generated />
using System;
using EbookArchiver.Data.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EbookArchiver.Data.MySql.Migrations
{
    [DbContext(typeof(EbookArchiverDbContext))]
    [Migration("20211209041911_AddPublisherIds")]
    partial class AddPublisherIds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EbookArchiver.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("EbookArchiver.Models.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FolderId")
                        .HasColumnType("longtext");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("EbookArchiver.Models.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("FolderId")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsNotOwned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PublisherId")
                        .HasColumnType("longtext");

                    b.Property<int?>("SeriesId")
                        .HasColumnType("int");

                    b.Property<string>("SeriesIndex")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("TitleDuplicatesSeriesData")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("BookId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("SeriesId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("EbookArchiver.Models.Ebook", b =>
                {
                    b.Property<int>("EbookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<string>("DrmStrippedFileId")
                        .HasColumnType("longtext");

                    b.Property<string>("DrmStrippedFileName")
                        .HasColumnType("longtext");

                    b.Property<string>("EbookFileId")
                        .HasColumnType("longtext");

                    b.Property<int>("EbookFormat")
                        .HasColumnType("int");

                    b.Property<int>("EbookSource")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .HasColumnType("longtext");

                    b.Property<string>("PublisherISBN13")
                        .HasColumnType("longtext");

                    b.Property<string>("PublisherVersion")
                        .HasColumnType("longtext");

                    b.Property<string>("VendorBookIdentifier")
                        .HasColumnType("longtext");

                    b.Property<string>("VendorVersion")
                        .HasColumnType("longtext");

                    b.HasKey("EbookId");

                    b.HasIndex("AccountId");

                    b.HasIndex("BookId");

                    b.ToTable("Ebooks");
                });

            modelBuilder.Entity("EbookArchiver.Models.Series", b =>
                {
                    b.Property<int>("SeriesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PublisherId")
                        .HasColumnType("longtext");

                    b.HasKey("SeriesId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("EbookArchiver.Models.Book", b =>
                {
                    b.HasOne("EbookArchiver.Models.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EbookArchiver.Models.Series", "Series")
                        .WithMany()
                        .HasForeignKey("SeriesId");

                    b.Navigation("Author");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("EbookArchiver.Models.Ebook", b =>
                {
                    b.HasOne("EbookArchiver.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("EbookArchiver.Models.Book", "Book")
                        .WithMany("Ebooks")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("EbookArchiver.Models.Book", b =>
                {
                    b.Navigation("Ebooks");
                });
#pragma warning restore 612, 618
        }
    }
}
