using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public sealed class Vehicle : IVehicle
    {
        private List<IItem> _inventory;
        private List<IPerson> _people;

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

        public float Balance { get; private set; }

        public ReadOnlyCollection<IPerson> People
        {
            get { return new ReadOnlyCollection<IPerson>(_people); }
        }

        public RationLevel Ration { get; private set; }

        public TravelPace Pace
        {
            get { return CurrentGame.Time.CurrentSpeed; }
        }

        public RepairStatus RepairStatus { get; private set; }

        public uint DistanceTraveled { get; set; }

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
            if (!(Balance >= totalCost))
                return;

            Balance -= totalCost;
            _inventory.Add(item);
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
            Balance = 0.0f;
            _people = new List<IPerson>();
            Ration = RationLevel.Filling;
            RepairStatus = RepairStatus.Good;
            DistanceTraveled = 0;
        }
    }
}