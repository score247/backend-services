using MessagePack;

namespace Soccer.API._Shared.Results
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CommandResult
    {
        public CommandResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }
}