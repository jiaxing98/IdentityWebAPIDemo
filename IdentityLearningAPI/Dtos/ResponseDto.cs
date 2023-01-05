namespace IdentityLearningAPI.Dtos
{
    public enum Status
    {
        Failure = 0,
        Success = 1,
    }

    public class ResponseDto
    {
        public Status Status { get; set; } = Status.Success;
        public object Result { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}