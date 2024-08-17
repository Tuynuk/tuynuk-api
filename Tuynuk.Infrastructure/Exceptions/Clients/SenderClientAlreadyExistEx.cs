using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Clients
{
    public class SenderClientAlreadyExistEx : BusinessLogicEx
    {
        public override string ErrorCode => "SenderClientAlreadyExist";
        public SenderClientAlreadyExistEx(string message) : base(message) { }
    }
}
