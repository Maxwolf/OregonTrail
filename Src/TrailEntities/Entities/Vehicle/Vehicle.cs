using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public sealed class Vehicle : IVehicle
    {
        private uint _distanceTraveled;
        private List<IItem> _inventory;
        private List<IPerson> _people;
        private RationLevel _ration;
        private RepairStatus _repairStatus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Vehicle" /> class.
        /// </summary>
        public Vehicle(IGameSimulation game)
        {
            CurrentGame = game;
            ResetVehicle(0);
        }

        public IGameSimulation CurrentGame { get; }

        public ReadOnlyCollection<IItem> Inventory
        {
            get { return new ReadOnlyCollection<IItem>(_inventory); }
        }

        public uint Balance { get; private set; }

        public ReadOnlyCollection<IPerson> People
        {
            get { return new ReadOnlyCollection<IPerson>(_people); }
        }

        public RationLevel Ration
        {
            get { return _ration; }
        }

        public TravelPace Pace
        {
            get { return CurrentGame.Time.CurrentSpeed; }
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
            _people.Add(person);
        }

        public void AddItem(IItem item)
        {
            _inventory.Add(item);
        }

        /// <summary>
        ///     Adds the item to the inventory of the vehicle and subtracts it's cost multiplied by quantity from balance.
        /// </summary>
        public void BuyItem(IItem item)
        {
            var totalCost = item.Cost*item.Quantity;
            if (Balance >= totalCost)
            {
                Balance -= totalCost;
                _inventory.Add(item);
            }
        }

        /// <summary>
        ///     Removes the item from the inventory of the vehicle and adds it's cost multiplied by quantity to balance.
        /// </summary>
        public void SellItem(IItem item)
        {
            var totalEarnings = item.Cost*item.Quantity;
            Balance += totalEarnings;
            _inventory.Remove(item);
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }

        public void ResetVehicle(uint startingMonies)
        {
            _inventory = new List<IItem>();
            Balance = 0;
            _people = new List<IPerson>();
            _ration = RationLevel.Filling;
            _repairStatus = RepairStatus.Good;
            _distanceTraveled = 0;
        }
    }
}