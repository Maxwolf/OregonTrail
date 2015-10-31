namespace TrailCommon
{
    public interface ITravelingMode : IMode
    {
        void ContinueOnTrail();
        void CheckSupplies();
        void LookAtMap();
        void ChangePace();
        void ChangeFoodRations();
        void StopToRest();
        void AttemptToTrade();
        void Hunt();
        void BuySupplies();
        void TalkToPeople();
    }
}