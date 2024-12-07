using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SareeDesigns.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: 'Saree',
                columns: table => new
                {
                    Id = table.Column<int>(type: 'integer', nullable: false)
                        .Annotation('Npgsql:ValueGenerationStrategy', NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: 'text', nullable: false),
                    Price = table.Column<int>(type: 'integer', nullable: false),
                    SavedUrl = table.Column<string>(type: 'text', nullable: true),
                    SavedFileName = table.Column<string>(type: 'text', nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey('PK_Saree', x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: 'Saree');
        }
    }
}
