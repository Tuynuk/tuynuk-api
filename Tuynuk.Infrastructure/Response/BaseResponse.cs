
namespace Tuynuk.Infrastructure.Response
{
    public class BaseResponse
    {
        public bool IsSuccess => Error == null;
        public object Data { get; set; }
        public BaseError Error { get; set; }
    }
}
