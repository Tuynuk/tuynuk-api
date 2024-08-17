
using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Sessions
{
    public class SessionNotFoundEx : BusinessLogicEx
    {
        public override string ErrorCode => "SessionNotFound";
        public SessionNotFoundEx(string message) : base(message) { }
    }
}
