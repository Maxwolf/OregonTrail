namespace TrailCommon
{
    public interface IRiver
    {
        uint Depth { get; } 
        uint FerryCost { get; }
        void CrossRiver();
    }
}