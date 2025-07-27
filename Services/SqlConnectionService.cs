using Microsoft.Data.SqlClient;

namespace BaiTap1.Services
{
    public class SqlConnectionService
    {
        public SqlConnection Connection { get; }

        public SqlConnectionService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("_connectionString");
            Connection = new SqlConnection(connectionString);
            Connection.Open(); // Mở kết nối ngay khi khởi tạo ứng dụng
        }
    }
}
