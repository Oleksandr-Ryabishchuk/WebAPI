namespace WebAPI.Exceptions
{
    public class SecureException: Exception
    {
        public SecureException(string message, int eventId)
        {
            CustomData = new CustomData(message, eventId);
        }
        public CustomData CustomData { get; set; }
    }

    public class CustomData
    {
        public CustomData(string message, int eventId)
        {
            Message = message;
            EventId = eventId;
        }
        public string Message { get; set; } = string.Empty;
        public int? EventId { get; set; }
    }
}
