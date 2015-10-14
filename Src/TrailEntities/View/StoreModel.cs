using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public class StoreModel : IStore
    {
        public IVehicle Vehicle
        {
            get { throw new System.NotImplementedException(); }
        }

        public SortedSet<IItem> Inventory
        {
            get { throw new System.NotImplementedException(); }
        }

        public uint Balance
        {
            get { throw new System.NotImplementedException(); }
        }

        public void BuyItems(IItem item)
        {
            throw new System.NotImplementedException();
        }

        public void SellItem(IItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}