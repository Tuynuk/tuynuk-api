using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Clients
{
    public class ReceiverClientNotFoundEx : BusinessLogicEx
    {
        public ReceiverClientNotFoundEx(string message) : base(message) { }

        public override string ErrorCode => "ReceiverClientNotFound";
    }
}
