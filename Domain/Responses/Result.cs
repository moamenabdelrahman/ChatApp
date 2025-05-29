namespace Domain.Responses
{
    public class Result<TData>
    {
        public bool Succeeded { get; private set; }

        public TData Data { get; private set; }

        public List<string> Errors { get; private set; }

        public static Result<TData> Ok() => 
            new Result<TData> { Succeeded = true, Data = default, Errors = new List<string>() };

        public static Result<TData> Ok(TData data) =>
            new Result<TData> { Succeeded = true, Data = data, Errors = new List<string>() };

        public static Result<TData> Fail() =>
            new Result<TData> { Succeeded = false, Data = default, Errors = new List<string>() };

        public static Result<TData> Fail(string message) =>
            new Result<TData> { Succeeded = false, Data = default, Errors = new List<string>() { message } };

        public static Result<TData> Fail(List<string> messages) =>
            new Result<TData> { Succeeded = false, Data = default, Errors = messages };
    }

    public class Result
    {
        public bool Succeeded { get; private set; }

        public object Data { get; private set; }

        public List<string> Errors { get; private set; }

        public static Result Ok() =>
            new Result { Succeeded = true, Data = default, Errors = new List<string>() };

        public static Result Fail() =>
            new Result { Succeeded = false, Data = default, Errors = new List<string>() };

        public static Result Fail(string message) =>
            new Result { Succeeded = false, Data = default, Errors = new List<string>() { message } };

        public static Result Fail(List<string> messages) =>
            new Result { Succeeded = false, Data = default, Errors = messages };
    }
}
