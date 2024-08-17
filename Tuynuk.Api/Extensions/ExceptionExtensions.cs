using Tuynuk.Infrastructure.Exceptions.Base;
using Tuynuk.Infrastructure.Response;

namespace Tuynuk.Api.Extensions
{
    public static class ExceptionExtensions
    {
        public static BaseResponse ToBaseResponse(this BusinessLogicEx ex)
        {
            return new BaseResponse
            {
                Error = new BaseError
                {
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message
                }
            };
        }

        public static BaseError ToBaseError(this BusinessLogicEx ex)
        {
            return new BaseError
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message
            };
        }

        public static BaseError ToBaseError(this Exception ex)
        {
            return new BaseError
            {
                ErrorCode = "Unexpected",
                Message = ex.Message
            };
        }
    }
}
