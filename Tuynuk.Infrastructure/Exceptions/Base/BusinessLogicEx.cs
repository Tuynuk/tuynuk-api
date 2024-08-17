
namespace Tuynuk.Infrastructure.Exceptions.Base
{
    public abstract class BusinessLogicEx : Exception
    {
        protected BusinessLogicEx(string message) : base(message) { }

        public abstract string ErrorCode { get; }
    }
}
