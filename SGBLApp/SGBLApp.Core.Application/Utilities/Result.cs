
namespace SGBLApp.Core.Application.Utilities
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public List<string> ValidationErrors { get; } = new();

        protected Result(T value, bool isSuccess, string error, List<string> validationErrors = null)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
            ValidationErrors = validationErrors ?? new List<string>();
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, null);
        public static Result<T> Failure(string error) => new Result<T>(default, false, error);
        public static Result<T> ValidationError(List<string> errors) => new Result<T>(default, false, "Validation errors", errors);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public List<string> ValidationErrors { get; } = new();

        protected Result(bool isSuccess, string error, List<string> validationErrors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            ValidationErrors = validationErrors ?? new List<string>();
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);
        public static Result ValidationError(List<string> errors) => new Result(false, "Validation errors", errors);
    }
}
