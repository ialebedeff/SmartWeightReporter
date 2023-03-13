using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class OperationResult
    {
        public bool Succeed { get; set; }
        public string? ErrorMessage { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
        public static OperationResult Ok()
            => new OperationResult() { Succeed = true };
        public static OperationResult Failed(string? message = null)
            => new OperationResult() { Succeed = false, ErrorMessage = message };
        public static OperationResult Failed(IEnumerable<IdentityError> identityErrors)
            => new OperationResult() { Succeed = false, Errors = identityErrors };
        public static OperationResult FromIdentityResult(IdentityResult identityResult)
            => new OperationResult { Succeed = identityResult.Succeeded, Errors = identityResult.Errors };
    }

    public class OperationResult<T>
    {
        public bool Succeed { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Result { get; set; }
        public static OperationResult<T> Ok(T result)
            => new OperationResult<T>() { Succeed = true, Result = result };
        public static OperationResult<T> Failed(string? message = null)
            => new OperationResult<T>() { Succeed = false, ErrorMessage = message };
    }
}