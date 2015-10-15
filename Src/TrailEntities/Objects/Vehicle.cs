using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public class Vehicle : IVehicle
    {
        private uint _balance;
        private uint _distanceTraveled;
        private SortedSet<IItem> _inventory;
        private TravelPace _pace;
        private SortedSet<IPerson> _people;
        private RationLevel _ration;
        private RepairStatus _repairStatus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Vehicle" /> class.
        /// </summary>
        public Vehicle()
        {
            _inventory = new SortedSet<IItem>();
            _balance = 0;
            _people = new SortedSet<IPerson>();
            _ration = RationLevel.Filling;
            _pace = TravelPace.Steady;
            _repairStatus = RepairStatus.Good;
            _distanceTraveled = 0;
        }

        public SortedSet<IItem> Inventory
        {
            get { return _inventory; }
        }

        public uint Balance
        {
            get { return _balance; }
        }

        public SortedSet<IPerson> People
        {
            get { return _people; }
        }

        public RationLevel Ration
        {
            get { return _ration; }
        }

        public TravelPace Pace
        {
            get { return _pace; }
        }

        public RepairStatus RepairStatus
        {
            get { return _repairStatus; }
        }

        public uint DistanceTraveled
        {
            get { return _distanceTraveled; }
            set { _distanceTraveled = value; }
        }

        public void AddPerson(IPerson person)
        {
            throw new NotImplementedException();
        }

        public void AddItem(IItem item)
        {
            throw new NotImplementedException();
        }

        public void BuyItem(IItem item)
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }
    }
}