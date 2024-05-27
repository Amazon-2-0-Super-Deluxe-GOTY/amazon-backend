namespace amazon_backend.Models
{
    public class Result<T>
    {
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public int pagesCount { get; set; }
        public int statusCode { get; set; }
        public T data { get; set; }
        public Result()
        {
            
        }
        public Result(string error)
        {
            message = error;
            isSuccess = false;
        }
        public Result(T data)
        {
            this.data = data;
            isSuccess = true;
        }
        public Result(T data, int pagesCount)
        {
            this.data = data;
            this.pagesCount = pagesCount;
            isSuccess = true;
        }
    }
}
