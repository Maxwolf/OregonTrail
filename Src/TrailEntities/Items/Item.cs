using System;
using TrailCommon;

namespace TrailEntities
{
    public abstract class Item : IItem
    {
        private uint _cost;
        private string _name;
        private uint _quantity;
        private uint _weight;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected Item(string name, uint quantity, uint weight, uint cost)
        {
            _name = name;
            _quantity = quantity;
            _weight = weight;
            _cost = cost;
        }

        public uint Cost
        {
            get { return _cost; }
        }

        public string Name
        {
            get { return _name; }
        }

        public uint Weight
        {
            get { return _weight; }
        }

        public uint Quantity
        {
            get { return _quantity; }
        }

        public virtual void Buy(int amount)
        {
            throw new NotImplementedException();
        }

        public uint TotalWeight()
        {
            throw new NotImplementedException();
        }
    }
}