using BaiTap1.Models;
using BaiTap1.Models.APIResponseModels;
using BaiTap1.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BaiTap1.Handlers
{
    public class BookHandler
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<AuthorHandler> _logger;

        public BookHandler(ILogger<AuthorHandler> logger, SqlConnectionService manager)
        {
            _connection = manager.Connection;
            _logger = logger;
        }

        public JsonResponseModel GetBookList()
        {
            _logger.LogInformation("Đang xử lý request lấy danh sách book.");

            try
            {
                var sqlString = "SELECT * FROM Books";
                var bookList = _connection.Query<BookModel>(sqlString).ToList();
                return new JsonResponseModel(true, 200, "OK", bookList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetBookList: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel GetBookById(string id)
        {
            _logger.LogInformation("Đang xử lý request lấy thông tin book theo id.");

            try
            {
                var sqlString = $"SELECT * FROM Books WHERE Id = '{id}'";
                var book = _connection.Query<BookModel>(sqlString).FirstOrDefault();

                if (book == null)
                {
                    return new JsonResponseModel(false, 404, "Not Found", null);
                }

                return new JsonResponseModel(true, 200, "OK", book);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetBookById: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }
        public JsonResponseModel GetBookByAuthorId(string id)
        {
            _logger.LogInformation("Đang xử lý request lấy thông tin book theo id của tác giả.");

            try
            {
                var sqlString = $"SELECT * FROM Books WHERE AuthorId = '{id}'";
                var book = _connection.Query<BookModel>(sqlString).ToList();

                if (book == null)
                {
                    return new JsonResponseModel(false, 404, "Not Found", null);
                }

                return new JsonResponseModel(true, 200, "OK", book);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string GetBookByAuthorId: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel InsertNewBook(BookModel bookNewInfo)
        {
            _logger.LogInformation("Đang xử lý request Thêm thông tin sách.");

            try
            {
                var isExistBook = CheckBookIsExist(bookNewInfo);
                if (isExistBook)
                {
                    _logger.LogWarning("Sách đã tồn tại trong hệ thống.");
                    return new JsonResponseModel(false, 409, "Conflict: Book is already exist", null);
                }

                bookNewInfo.Id = Guid.NewGuid(); // Tạo Id mới cho sách

                var sqlString = "INSERT INTO Books (Id, Title, AuthorId) VALUES (@Id, @Title, @AuthorId)";
                _connection.Execute(sqlString, new { Title = bookNewInfo.Title, AuthorId = bookNewInfo.AuthorId, Id = bookNewInfo.Id });

                // Trả về bản ghi sau khi cập nhật
                return new JsonResponseModel(true, 200, "OK", bookNewInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string InsertNewBook: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }


        public JsonResponseModel UpdateBookById(string id, BookModel bookUpdateInfo)
        {
            _logger.LogInformation("Đang xử lý request cập nhật thông tin book theo id.");

            try
            {
                JsonResponseModel bookInfoById = GetBookById(id);
                if (bookInfoById.Code == 404)
                {
                    return bookInfoById;
                }

                var isExistBook = CheckBookIsExist(bookUpdateInfo);
                if (isExistBook)
                {
                    _logger.LogWarning("Sách đã tồn tại trong hệ thống.");
                    return new JsonResponseModel(false, 409, "Conflict: Book is already exist", null);
                }

                var sqlString = "UPDATE Books SET AuthorId = @AuthorId, Title = @Title WHERE Id = @Id";
                _connection.Execute(sqlString, new { AuthorId = bookUpdateInfo.AuthorId, Title = bookUpdateInfo.Title, Id = id });

                bookUpdateInfo.Id = new Guid(id); // Cập nhật Id cho bookUpdateInfo

                return new JsonResponseModel(true, 200, "OK", bookUpdateInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string UpdateBookById: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }
        public JsonResponseModel DeleteBookById(string id)
        {
            _logger.LogInformation("Đang xử lý request xóa book theo id.");

            try
            {
                JsonResponseModel bookInfoById = GetBookById(id);

                if (bookInfoById.Code == 404)
                {
                    return bookInfoById;
                }

                var sqlString = $"DELETE FROM Books WHERE Id = '{id}'";
                _connection.Execute(sqlString);

                return new JsonResponseModel(true, 200, "OK", null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string DeleteBookById: " + ex);
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        // Kiểm tra xem có sách cùng tên cùng tác giả đã tồn tại hay chưa
        public bool CheckBookIsExist(BookModel bookCheckExistInfo)
        {
            _logger.LogInformation("Đang xử lý request kiểm tra thông tin sách tồn tại hay chưa.");

            try
            {
                var sqlString = $"SELECT * FROM Books WHERE Title = N'{bookCheckExistInfo.Title}' and AuthorId = N'{bookCheckExistInfo.AuthorId}'";
                var book = _connection.Query<BookModel>(sqlString).FirstOrDefault();

                if (book == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi call sql string CheckBookIsExist: " + ex);

                return true;
            }
        }
    }
}
