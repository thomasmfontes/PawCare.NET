namespace PawCareApi.Domain.Common
{
    public class OperationResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public int StatusCode { get; }
        public IReadOnlyList<string> Errors { get; }

        protected OperationResult(bool isSuccess, string message, int statusCode, IEnumerable<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
            Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        }

        public static OperationResult Success(string message = "Operação realizada com sucesso.", int statusCode = 200)
            => new(true, message, statusCode);

        public static OperationResult Created(string message = "Recurso criado com sucesso.")
            => new(true, message, 201);

        public static OperationResult NoContent(string message = "Operação realizada com sucesso.")
            => new(true, message, 204);

        public static OperationResult Failure(string message, int statusCode = 400, params string[] errors)
            => new(false, message, statusCode, errors);

        public static OperationResult NotFound(string message = "Recurso não encontrado.")
            => new(false, message, 404);

        public static OperationResult Unauthorized(string message = "Não autorizado.")
            => new(false, message, 401);

        public static OperationResult Forbidden(string message = "Acesso negado.")
            => new(false, message, 403);

        public static OperationResult Conflict(string message = "Conflito com o estado atual do recurso.")
            => new(false, message, 409);
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; }

        protected OperationResult(bool isSuccess, T? data, string message, int statusCode, IEnumerable<string>? errors = null)
            : base(isSuccess, message, statusCode, errors)
        {
            Data = data;
        }

        public static OperationResult<T> Success(T data, string message = "Operação realizada com sucesso.", int statusCode = 200)
            => new(true, data, message, statusCode);

        public static OperationResult<T> Created(T data, string message = "Recurso criado com sucesso.")
            => new(true, data, message, 201);

        public static new OperationResult<T> Failure(string message, int statusCode = 400, params string[] errors)
            => new(false, default, message, statusCode, errors);

        public static new OperationResult<T> NotFound(string message = "Recurso não encontrado.")
            => new(false, default, message, 404);

        public static new OperationResult<T> Unauthorized(string message = "Não autorizado.")
            => new(false, default, message, 401);

        public static new OperationResult<T> Forbidden(string message = "Acesso negado.")
            => new(false, default, message, 403);

        public static new OperationResult<T> Conflict(string message = "Conflito com o estado atual do recurso.")
            => new(false, default, message, 409);
    }
}

