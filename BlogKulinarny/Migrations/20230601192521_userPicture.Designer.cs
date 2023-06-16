﻿// <auto-generated />
using System;
using BlogKulinarny.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlogKulinarny.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230601192521_userPicture")]
    partial class userPicture
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BlogKulinarny.Models.Category", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("BlogKulinarny.Models.Recipe", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("avgTime")
                        .HasColumnType("int");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("difficulty")
                        .HasColumnType("int");

                    b.Property<string>("imageURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("bit");

                    b.Property<int>("portions")
                        .HasColumnType("int");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("userId");

                    b.ToTable("recipes");
                });

            modelBuilder.Entity("BlogKulinarny.Models.RecipeElements", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imageURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("noOfList")
                        .HasColumnType("int");

                    b.Property<int>("recipeId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("recipeId");

                    b.ToTable("recipesElements");
                });

            modelBuilder.Entity("BlogKulinarny.Models.RecipesCategory", b =>
                {
                    b.Property<int>("recipeId")
                        .HasColumnType("int");

                    b.Property<int>("categoryId")
                        .HasColumnType("int");

                    b.HasKey("recipeId", "categoryId");

                    b.HasIndex("categoryId");

                    b.ToTable("recipesCategories");
                });

            modelBuilder.Entity("BlogKulinarny.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("imageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("bit");

                    b.Property<string>("login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("rank")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("BlogKulinarny.Models.Recipe", b =>
                {
                    b.HasOne("BlogKulinarny.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("BlogKulinarny.Models.RecipeElements", b =>
                {
                    b.HasOne("BlogKulinarny.Models.Recipe", "recipe")
                        .WithMany()
                        .HasForeignKey("recipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("recipe");
                });

            modelBuilder.Entity("BlogKulinarny.Models.RecipesCategory", b =>
                {
                    b.HasOne("BlogKulinarny.Models.Category", "category")
                        .WithMany("recipesCategories")
                        .HasForeignKey("categoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogKulinarny.Models.Recipe", "recipe")
                        .WithMany("recipesCategories")
                        .HasForeignKey("recipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("category");

                    b.Navigation("recipe");
                });

            modelBuilder.Entity("BlogKulinarny.Models.Category", b =>
                {
                    b.Navigation("recipesCategories");
                });

            modelBuilder.Entity("BlogKulinarny.Models.Recipe", b =>
                {
                    b.Navigation("recipesCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
