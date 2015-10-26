namespace TrailCommon
{
    public interface ITravelingMode : IMode
    {
        bool CanRest { get; }
        void GoToStore();
        void ContinueOnTrail();
        void CheckSupplies();
        void LookAtMap();
        void ChangePace();
        void ChangeFoodRations();
        void StopToRest();
        void AttemptToTrade();
        void TalkToPeople();
        void BuySupplies();
        void Hunt();
    }
}