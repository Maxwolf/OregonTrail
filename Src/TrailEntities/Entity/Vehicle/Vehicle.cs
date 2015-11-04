using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrailEntities
{
    /// <summary>
    ///     Vessel that holds all the players, their inventory, money, and keeps track of total miles traveled in the form of
    ///     an odometer.
    /// </summary>
    public sealed class Vehicle : IEntity
    {
        public delegate void OnChangePace();

        public delegate void OnChangeRation();

        private HashSet<Item> _inventory;
        private HashSet<Person> _passengers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Vehicle" /> class.
        /// </summary>
        public Vehicle()
        {
            ResetVehicle(0);
            Name = "Vehicle";
            Pace = TravelPace.Steady;
        }

        public IEnumerable<Item> Inventory
        {
            get { return _inventory; }
        }

        public float Balance { get; private set; }

        public IEnumerable<Person> Passengers
        {
            get { return _passengers; }
        }

        public RationLevel Ration { get; private set; }

        public TravelPace Pace { get; private set; }

        public RepairStatus RepairStatus { get; private set; }

        public ulong Odometer { get; set; }

        /// <summary>
        ///     Name of the entity as it should be known in the simulation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IEntity other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (Name.Equals(other.Name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
        }

        public event OnChangePace OnVehicleChangePace;

        public event OnChangeRation OnVehicleChangeRations;

        /// <summary>
        ///     Sets the current speed of the game simulation.
        /// </summary>
        public void ChangePace(TravelPace castedSpeed)
        {
            // Check to make sure we are not already at this speed.
            if (castedSpeed == Pace)
                return;

            // Change game simulation speed.
            Pace = castedSpeed;

            // Inform subscribers we updated progression of time.
            OnVehicleChangePace?.Invoke();
        }

        /// <summary>
        ///     Adds a new person object to the list of vehicle passengers.
        /// </summary>
        /// <param name="person">Person that wishes to become a vehicle passenger.</param>
        public void AddPerson(Person person)
        {
            _passengers.Add(person);
        }

        /// <summary>
        ///     Adds the item to the inventory of the vehicle and subtracts it's cost multiplied by quantity from balance.
        /// </summary>
        public void BuyItem(StoreTransactionItem transaction)
        {
            var totalCost = transaction.Item.Cost*transaction.Quantity;
            if (!(Balance >= totalCost))
                return;

            Balance -= totalCost;
            _inventory.Add(transaction.Item);
        }

        /// <summary>
        ///     Removes the item from the inventory of the vehicle and adds it's cost multiplied by quantity to balance.
        /// </summary>
        public void SellItem(StoreTransactionItem transaction)
        {
            var totalEarnings = transaction.Item.Cost*transaction.Quantity;
            Balance += totalEarnings;
            _inventory.Remove(transaction.Item);
        }

        public void ResetVehicle(uint startingMonies)
        {
            _inventory = new HashSet<Item>();
            Balance = startingMonies;
            _passengers = new HashSet<Person>();
            Ration = RationLevel.Filling;
            RepairStatus = RepairStatus.Good;
            Odometer = 0;
        }

        public void TickVehicle()
        {
            
        }

        /// <summary>
        ///     Changes the current ration level to new value if it is not already set to that. Also fires even about this for
        ///     subscribers to get event notification about the change.
        /// </summary>
        /// <param name="ration">The rate at which people are permitted to eat in the vehicle party.</param>
        public void ChangeRations(RationLevel ration)
        {
            // Ensure we are actually changing it to something else.
            if (ration == Ration)
                return;

            // Set new ration level.
            Ration = ration;

            // Fire event so subscribers know we changed rations.
            OnVehicleChangeRations?.Invoke();
        }
    }
}