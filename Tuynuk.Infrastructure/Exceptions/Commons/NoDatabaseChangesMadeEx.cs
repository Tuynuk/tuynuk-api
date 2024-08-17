using Tuynuk.Infrastructure.Exceptions.Base;

namespace Tuynuk.Infrastructure.Exceptions.Commons
{
    public class NoDatabaseChangesMadeEx : BusinessLogicEx
    {
        public override string ErrorCode => "NoDatabaseChangesMade";
        public NoDatabaseChangesMadeEx(string message) : base(message) { }
    }
}
