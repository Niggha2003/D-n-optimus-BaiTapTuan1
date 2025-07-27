using BaiTap1.Handlers;
using BaiTap1.Models;
using BaiTap1.Models.APIResponseModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BaiTap1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookHandler _bookHandler;

        public BookController(BookHandler bookHandler)
        {
            _bookHandler = bookHandler;
        }

        [Route("getBookList")]
        [HttpGet]
        public JsonResponseModel GetBookList()
        {
            return _bookHandler.GetBookList();
        }

        [Route("getBookById")]
        [HttpPost]
        public JsonResponseModel GetBookById([FromBody] string id)
        {
            return _bookHandler.GetBookById(id);
        }

        [Route("getBookByAuthorId")]
        [HttpPost]
        public JsonResponseModel GetBookByAuthorId([FromBody] string id)
        {
            return _bookHandler.GetBookByAuthorId(id);
        }

        [Route("createNewBook")]
        [HttpPost]
        public JsonResponseModel createNewBook([FromBody] BookModel request)
        {
            return _bookHandler.InsertNewBook(request);
        }

        [Route("updateBookById")]
        [HttpPost]
        public JsonResponseModel UpdateBookById(BookModel bookInfo)
        {

            return _bookHandler.UpdateBookById(bookInfo.Id.ToString(), bookInfo);
        }

        [Route("deleteBookById")]
        [HttpPost]
        public JsonResponseModel DeleteBookById([FromBody] string id)
        {
            return _bookHandler.DeleteBookById(id);
        }
    }
}
