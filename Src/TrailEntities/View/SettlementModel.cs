using TrailCommon;

namespace TrailEntities
{
    public class SettlementModel : ISettlement
    {
        public bool CanRest
        {
            get { throw new System.NotImplementedException(); }
        }

        public IStore Store
        {
            get { throw new System.NotImplementedException(); }
        }

        public void GoToStore()
        {
            throw new System.NotImplementedException();
        }
    }
}