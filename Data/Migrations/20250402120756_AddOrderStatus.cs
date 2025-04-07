using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
         name: "TempStatus",
         table: "Orders",
         type: "nvarchar(max)",
         nullable: true);

            // 2. Copy dữ liệu từ cột Status sang TempStatus
            migrationBuilder.Sql("UPDATE [Orders] SET [TempStatus] = [Status]");

            // 3. Xóa cột Status cũ
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // 4. Tạo lại cột Status với kiểu int (để map enum)
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                nullable: false,
                defaultValue: 0); // 0 = OrderPlaced

            // 5. Convert giá trị từ string sang int
            migrationBuilder.Sql(@"
        UPDATE [Orders] 
        SET [Status] = 
            CASE 
                WHEN [TempStatus] = 'Order Placed' THEN 0
                WHEN [TempStatus] = 'Payment Pending' THEN 1
                WHEN [TempStatus] = 'Shipped' THEN 2
                WHEN [TempStatus] = 'Delivered' THEN 3
                ELSE 0
            END
    ");

            // 6. Xóa cột tạm
            migrationBuilder.DropColumn(
                name: "TempStatus",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
