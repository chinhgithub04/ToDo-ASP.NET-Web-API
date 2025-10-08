namespace ToDo.Application.DTOs.Common
{
    public record ResponseDto<T>
    {
        public T Data { get; init; }
        public bool IsSuccess { get; init; }
        public string Message { get; init; }
        public int StatusCode { get; init; }
        public List<string> Errors { get; init; }
    }
}
