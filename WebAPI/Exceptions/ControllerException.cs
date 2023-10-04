namespace WebAPI.Exceptions
{
    public class ControllerException : Exception
    {
        public ControllerException(string message, int eventId)
        {
            CustomData = new CustomData(message, eventId);
        }
        public CustomData CustomData { get; set; }
    }    
}
