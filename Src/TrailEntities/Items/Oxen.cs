using System;
using TrailCommon;

namespace TrailEntities
{
    public class Oxen : Item
    {
        private uint _grassAvaliable;

        private RepairStatus _oxenHealth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public Oxen(string name, uint quantity, uint weight, uint cost, uint grassAvaliable, RepairStatus oxenHealth)
            : base(name, quantity, weight, cost)
        {
            _grassAvaliable = grassAvaliable;
            _oxenHealth = oxenHealth;
        }

        public uint GrassAvaliable
        {
            get { return _grassAvaliable; }
        }

        public RepairStatus OxenHealth
        {
            get { return _oxenHealth; }
        }

        public void Eat()
        {
            throw new NotImplementedException();
        }

        public void Die()
        {
            throw new NotImplementedException();
        }
    }
}