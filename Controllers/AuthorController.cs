using BaiTap1.Handlers;
using BaiTap1.Models;
using BaiTap1.Models.APIResponseModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BaiTap1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorHandler _authorHandler;

        public AuthorController(AuthorHandler authorHandler)
        {
            _authorHandler = authorHandler;
        }

        [Route("getAuthorList")]
        [HttpGet]
        public JsonResponseModel GetAuthorList()
        {
            var result = _authorHandler.GetAuthorList();
            return result;
        }

        [Route("getAuthorById")]
        [HttpPost]
        public JsonResponseModel GetAuthorById([FromBody] string id)
        {
            return _authorHandler.GetAuthorById(id);
        }

        [Route("createNewAuthor")]
        [HttpPost]
        public JsonResponseModel CreateNewAuthor([FromBody] AuthorModel request)
        {
            return _authorHandler.InsertNewAuthor(request);
        }

        [Route("updateAuthorById")]
        [HttpPost]
        public JsonResponseModel UpdateAuthorById([FromBody] AuthorModel request)
        {
            return _authorHandler.UpdateAuthorById(request.Id.ToString(), request);
        }

        [Route("deleteAuthorById")]
        [HttpPost]
        public JsonResponseModel DeleteAuthorById([FromBody] string id)
        {
            return _authorHandler.DeleteAuthorById(id);
        }
    }
}
