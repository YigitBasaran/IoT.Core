﻿// <auto-generated />
using System;
using IoT.Core.AuthService.Repo.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IoT.Core.AuthService.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IoT.Core.AuthService.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PasswordHash = "$2a$11$938geM.vJ0pq5raKUfMBB.y0Wq6MSuIefVoptpY5kFEVI4Palx2WC",
                            Role = 0,
                            Username = "admin"
                        },
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PasswordHash = "$2a$11$djNSsxIQuzCrU1AQktDX4uqysqnNLaX9OhLDdq5ZUIXW7qWXPC2na",
                            Role = 1,
                            Username = "Tesla"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PasswordHash = "$2a$11$Gw1V.HcCVJamgzWmKyzO6O/8WoSJrzK5oO9gAthNWTXLt3iaE2GHq",
                            Role = 1,
                            Username = "Google"
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PasswordHash = "$2a$11$nImEvlk9fjvOLY9Qkqkdm.MAbvaH1li6jpm3J7jl1n5EHHzWa8xSK",
                            Role = 1,
                            Username = "Amazon"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
