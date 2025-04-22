namespace wspay_v2.Result
{
    /// <summary>
    /// Generic class to return a result with a value or not
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class Result<TResponse>
    {
        public bool IsSuccess => Value != null;
        public string Message { get; set; } = string.Empty;
        // Encapsulation
        public TResponse? Value { get; private set; }

        // Generic method to return a success or failure result
        public static Result<TResponse> Success<TResponse>(TResponse value)
        {
            return new Result<TResponse> { Value = value };
        }

        public static Result<TResponse> Failure()
        {
            return new Result<TResponse>();
        }

        public static Result<TResponse> Failure(string message)
        {
            return new Result<TResponse> { Message = message };
        }
    }
}
