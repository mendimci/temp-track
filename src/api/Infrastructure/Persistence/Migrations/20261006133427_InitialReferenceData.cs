using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TempTrack.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialReferenceData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "approval_chain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_approval_chain", x => x.id);
                table.UniqueConstraint("ak_approval_chain_code", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "department",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                directorate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_department", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "pay_rate",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                staff_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                band = table.Column<int>(type: "integer", nullable: false),
                hourly_rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_pay_rate", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "reason_code",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_reason_code", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "approval_chain_step",
            columns: table => new
            {
                chain_id = table.Column<Guid>(type: "uuid", nullable: false),
                step_order = table.Column<int>(type: "integer", nullable: false),
                role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_approval_chain_step", x => new { x.chain_id, x.step_order });
                table.ForeignKey(
                    name: "fk_approval_chain_step_approval_chain_chain_id",
                    column: x => x.chain_id,
                    principalTable: "approval_chain",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "routing_rule",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                priority = table.Column<int>(type: "integer", nullable: false),
                match_staff_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                match_min_cost = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                chain_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_routing_rule", x => x.id);
                table.ForeignKey(
                    name: "fk_routing_rule_approval_chain_chain_code",
                    column: x => x.chain_code,
                    principalTable: "approval_chain",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "app_user",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                external_id = table.Column<string>(type: "text", nullable: true),
                display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                home_department_id = table.Column<Guid>(type: "uuid", nullable: true),
                is_synthetic = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_app_user", x => x.id);
                table.ForeignKey(
                    name: "fk_app_user_department_home_department_id",
                    column: x => x.home_department_id,
                    principalTable: "department",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            name: "user_department_scope",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                department_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_department_scope", x => new { x.user_id, x.department_id });
                table.ForeignKey(
                    name: "fk_user_department_scope_app_user_user_id",
                    column: x => x.user_id,
                    principalTable: "app_user",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_department_scope_department_department_id",
                    column: x => x.department_id,
                    principalTable: "department",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_app_user_email",
            table: "app_user",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_app_user_external_id",
            table: "app_user",
            column: "external_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_app_user_home_department_id",
            table: "app_user",
            column: "home_department_id");

        migrationBuilder.CreateIndex(
            name: "ix_department_code",
            table: "department",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_pay_rate_staff_type_band",
            table: "pay_rate",
            columns: new[] { "staff_type", "band" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_routing_rule_chain_code",
            table: "routing_rule",
            column: "chain_code");

        migrationBuilder.CreateIndex(
            name: "ix_routing_rule_priority",
            table: "routing_rule",
            column: "priority",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_department_scope_department_id",
            table: "user_department_scope",
            column: "department_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "approval_chain_step");

        migrationBuilder.DropTable(
            name: "pay_rate");

        migrationBuilder.DropTable(
            name: "reason_code");

        migrationBuilder.DropTable(
            name: "routing_rule");

        migrationBuilder.DropTable(
            name: "user_department_scope");

        migrationBuilder.DropTable(
            name: "approval_chain");

        migrationBuilder.DropTable(
            name: "app_user");

        migrationBuilder.DropTable(
            name: "department");
    }
}
