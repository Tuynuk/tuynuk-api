
using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Files
{
    public class FileAlreadyRequestedEx : BusinessLogicEx
    {
        public override string ErrorCode => "FileAlreadyRequested";
        public FileAlreadyRequestedEx(string message) : base(message) { }
    }
}
