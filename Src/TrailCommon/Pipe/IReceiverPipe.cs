namespace TrailCommon
{
    public interface IReceiverPipe : IPipe
    {
        event ResponseReceived ResponseReceived;
    }
}