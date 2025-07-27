namespace BaiTap1.Models.APIResponseModels
{
    public class JsonResponseModel
    {
        public JsonResponseModel(bool status, int code, string? message, object? data) 
        { 
            Status = status;
            Code = code;
            Message = message;
            Data = data;
        }

        public bool Status { get; set; } // success or not
        public int Code { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
