namespace amazon_backend.Models
{
    public class Result<T>
    {
        public string message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public Result(string error)
        {
            message = error;
            IsSuccess = false;
        }
        public Result(T data)
        {
            Data = data;
            IsSuccess = true;
        }
    }
}
