using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIED_LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accounts_domain_users_domain_user_id",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "ix_accounts_domain_user_id",
                table: "accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_accounts_domain_user_id",
                table: "accounts",
                column: "domain_user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_domain_users_domain_user_id",
                table: "accounts",
                column: "domain_user_id",
                principalTable: "domain_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
