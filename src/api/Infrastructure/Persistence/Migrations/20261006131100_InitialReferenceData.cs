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
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_approval_chain", x => x.Id);
                table.UniqueConstraint("AK_approval_chain_Code", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "department",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Directorate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_department", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "pay_rate",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StaffType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                Band = table.Column<int>(type: "integer", nullable: false),
                HourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_pay_rate", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "reason_code",
            columns: table => new
            {
                Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_reason_code", x => x.Code);
            });

        migrationBuilder.CreateTable(
            name: "approval_chain_step",
            columns: table => new
            {
                ChainId = table.Column<Guid>(type: "uuid", nullable: false),
                StepOrder = table.Column<int>(type: "integer", nullable: false),
                Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_approval_chain_step", x => new { x.ChainId, x.StepOrder });
                table.ForeignKey(
                    name: "FK_approval_chain_step_approval_chain_ChainId",
                    column: x => x.ChainId,
                    principalTable: "approval_chain",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "routing_rule",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Priority = table.Column<int>(type: "integer", nullable: false),
                MatchStaffType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                MatchMinCost = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                ChainCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_routing_rule", x => x.Id);
                table.ForeignKey(
                    name: "FK_routing_rule_approval_chain_ChainCode",
                    column: x => x.ChainCode,
                    principalTable: "approval_chain",
                    principalColumn: "Code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "app_user",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalId = table.Column<string>(type: "text", nullable: true),
                DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                HomeDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                IsSynthetic = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_app_user", x => x.Id);
                table.ForeignKey(
                    name: "FK_app_user_department_HomeDepartmentId",
                    column: x => x.HomeDepartmentId,
                    principalTable: "department",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "user_department_scope",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                DepartmentId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_department_scope", x => new { x.UserId, x.DepartmentId });
                table.ForeignKey(
                    name: "FK_user_department_scope_app_user_UserId",
                    column: x => x.UserId,
                    principalTable: "app_user",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_user_department_scope_department_DepartmentId",
                    column: x => x.DepartmentId,
                    principalTable: "department",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_app_user_Email",
            table: "app_user",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_app_user_ExternalId",
            table: "app_user",
            column: "ExternalId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_app_user_HomeDepartmentId",
            table: "app_user",
            column: "HomeDepartmentId");

        migrationBuilder.CreateIndex(
            name: "IX_department_Code",
            table: "department",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_pay_rate_StaffType_Band",
            table: "pay_rate",
            columns: new[] { "StaffType", "Band" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_routing_rule_ChainCode",
            table: "routing_rule",
            column: "ChainCode");

        migrationBuilder.CreateIndex(
            name: "IX_routing_rule_Priority",
            table: "routing_rule",
            column: "Priority",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_user_department_scope_DepartmentId",
            table: "user_department_scope",
            column: "DepartmentId");
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
