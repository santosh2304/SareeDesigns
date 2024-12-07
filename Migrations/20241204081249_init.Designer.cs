﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SareeDesigns.Data;

#nullable disable

namespace SareeDesigns.Migrations
{
    [DbContext(typeof(SareeDesignsContext))]
    [Migration('20241204081249_init')]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation('ProductVersion', '8.0.11')
                .HasAnnotation('Relational:MaxIdentifierLength', 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity('SareeDesigns.Models.Saree', b =>
                {
                    b.Property<int>('Id')
                        .ValueGeneratedOnAdd()
                        .HasColumnType('integer');

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>('Id'));

                    b.Property<string>('Name')
                        .IsRequired()
                        .HasColumnType('text');

                    b.Property<int>('Price')
                        .HasColumnType('integer');

                    b.Property<string>('SavedFileName')
                        .HasColumnType('text');

                    b.Property<string>('SavedUrl')
                        .HasColumnType('text');

                    b.HasKey('Id');

                    b.ToTable('Saree');
                });
#pragma warning restore 612, 618
        }
    }
}
