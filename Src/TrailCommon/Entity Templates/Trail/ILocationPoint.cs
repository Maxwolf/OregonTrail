using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ILocationPoint
    {
        string Name { get; }
        bool CanRest { get; }
        bool HasStore { get; }
        float StoreBalance { get; }
        ReadOnlyCollection<Item> StoreItems { get; }
        void GoToStore();
    }
}