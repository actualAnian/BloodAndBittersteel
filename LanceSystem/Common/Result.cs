namespace LanceSystem.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }

        Result(T value) { IsSuccess = true; Value = value; }
        Result(string error) { IsSuccess = false; ErrorMessage = error; }

        public static Result<T> Ok(T value) => new(value);
        public static Result<T> Fail(string error) => new(error);
    }
}
