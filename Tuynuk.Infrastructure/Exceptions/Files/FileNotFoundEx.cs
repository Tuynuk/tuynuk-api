
using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Files
{
    public class FileNotFoundEx : BusinessLogicEx
    {
        public override string ErrorCode => "FileNotFound";
        public FileNotFoundEx(string message) : base(message) { }
    }
}
