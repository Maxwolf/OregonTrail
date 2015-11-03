namespace TrailEntities
{
    public interface IRiverPoint
    {
        uint Depth { get; }
        uint FerryCost { get; }
        void CrossRiver();
    }
}