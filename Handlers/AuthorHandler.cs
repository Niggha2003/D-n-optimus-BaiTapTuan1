using BaiTap1.Controllers;
using BaiTap1.Models;
using BaiTap1.Models.APIResponseModels;
using BaiTap1.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BaiTap1.Handlers
{
    public class AuthorHandler
    {
        private readonly SqlConnection _connection; 
        private readonly ILogger<AuthorHandler> _logger;

        public AuthorHandler(ILogger<AuthorHandler> logger, SqlConnectionService manager)
        {
            _connection = manager.Connection;
            _logger = logger;
        }

        public JsonResponseModel GetAuthorList()
        {
            _logger.LogInformation("Đang xử lý request lấy danh sách tác giả.");

            try
            {
                var sqlString = "SELECT * FROM Authors";
                var authorList = _connection.Query<AuthorModel>(sqlString).ToList();

                return new JsonResponseModel(true, 200, "OK", authorList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetAuthorList: " + ex);

                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel GetAuthorById(string id)
        {
            _logger.LogInformation("Đang xử lý request lấy thông tin tác giả theo id.");

            try
            {
                var sqlString = $"SELECT * FROM Authors WHERE Id = '{id}'";
                var author = _connection.Query<AuthorModel>(sqlString).FirstOrDefault();

                if (author == null) 
                { 
                    return new JsonResponseModel(false, 404, "Not Found", null);
                }

                return new JsonResponseModel(true, 200, "OK", author);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetAuthorById: " + ex);

                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel InsertNewAuthor(AuthorModel authorNewInfo)
        {
            _logger.LogInformation("Đang xử lý request Thêm thông tin tác giả.");

            try
            {
                var isExistAuthor = CheckAuthorIsExist(authorNewInfo);
                if (isExistAuthor)
                {
                    _logger.LogWarning("Tác giả đã tồn tại trong hệ thống.");
                    return new JsonResponseModel(false, 409, "Conflict: Author is already exist", null);
                }

                authorNewInfo.Id = Guid.NewGuid(); // Tạo Id mới cho tác giả

                var sqlString = "INSERT INTO Authors (Id, AuthorName) VALUES (@Id, @AuthorName)";
                _connection.Execute(sqlString, new { AuthorName = authorNewInfo.AuthorName, Id = authorNewInfo.Id });

                // Trả về bản ghi sau khi cập nhật
                return new JsonResponseModel(true, 200, "OK", authorNewInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string InsertNewAuthor: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel UpdateAuthorById(string id, AuthorModel authorUpdateInfo)
        {
            _logger.LogInformation("Đang xử lý request cập nhật thông tin tác giả theo id.");

            try
            {
                JsonResponseModel authorInfoById = GetAuthorById(id);
                if( authorInfoById.Code == 404 )
                {
                    return authorInfoById;
                }

                var isExistAuthor = CheckAuthorIsExist(authorUpdateInfo);
                if (isExistAuthor)
                {
                    _logger.LogWarning("Tác giả đã tồn tại trong hệ thống.");
                    return new JsonResponseModel(false, 409, "Conflict: Author is already exist", null);
                }

                var sqlString = "UPDATE Authors SET AuthorName = @AuthorName WHERE Id = @Id";
                _connection.Execute(sqlString, new { AuthorName = authorUpdateInfo.AuthorName, Id = id });

                authorUpdateInfo.Id = new Guid(id); // Cập nhật Id cho tác giả

                return new JsonResponseModel(true, 200, "OK", authorUpdateInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string UpdateAuthorById: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel DeleteAuthorById(string id)
        {
            _logger.LogInformation("Đang xử lý request xóa tác giả theo id.");

            try
            {
                JsonResponseModel authorInfoById = GetAuthorById(id);

                if (authorInfoById.Code == 404)
                {
                    return authorInfoById;
                }

                var bookByAuthorId = CheckIsExistBookByAuthorId(id);
                if (bookByAuthorId)
                {
                    _logger.LogWarning("Không thể xóa tác giả vì có sách liên kết với tác giả này.");
                    return new JsonResponseModel(false, 409, "Conflict: Author has books associated", null);
                }

                var sqlString = $"DELETE FROM Authors WHERE Id = '{id}'";
                _connection.Execute(sqlString);

                // Trả về null hoặc thông báo đã xóa
                return new JsonResponseModel(true, 200, "OK", null); ;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string DeleteAuthorById: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null); 
            }
        }

        // Kiểm tra xem có tác giả cùng tên đã được lưu trữ hay chưa
        public bool CheckAuthorIsExist(AuthorModel authorCheckExistInfo)
        {
            _logger.LogInformation("Đang xử lý request kiểm tra thông tin tác giả tồn tại hay chưa.");

            try
            {
                var sqlString = $"SELECT * FROM Authors WHERE AuthorName = N'{authorCheckExistInfo.AuthorName}'";
                var author = _connection.Query<AuthorModel>(sqlString).FirstOrDefault();

                if (author == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string CheckAuthorIsExist: " + ex);

                return true;
            }
        }

        // Kiểm tra có sách tồn tại theo id của tác giả hay không
        public bool CheckIsExistBookByAuthorId(string id)
        {
            _logger.LogInformation("Đang xử lý request lấy thông tin book theo id của tác giả.");

            try
            {
                var sqlString = $"SELECT * FROM Books WHERE AuthorId = '{id}'";
                var book = _connection.Query<BookModel>(sqlString).FirstOrDefault();

                if (book == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetBookByAuthorId: " + ex);
                return true;
            }
        }

    }

}
