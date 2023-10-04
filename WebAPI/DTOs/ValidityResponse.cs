namespace WebAPI.DTOs
{
    public class ValidityResponse(bool valid, string reason)
    {
        public bool Valid { get; set; } = valid;
        public string Reason { get; set; } = reason;
    }
}
