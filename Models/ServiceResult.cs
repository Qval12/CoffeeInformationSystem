namespace CoffeeInformationSystem.Models
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string ErrorMessage { get; }

        private ServiceResult(T data)
        {
            IsSuccess = true;
            Data = data;
            ErrorMessage = string.Empty;
        }

        private ServiceResult(string errorMessage)
        {
            IsSuccess = false;
            Data = default;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T>(data);
        public static ServiceResult<T> Failure(string errorMessage) => new ServiceResult<T>(errorMessage);
    }
}
