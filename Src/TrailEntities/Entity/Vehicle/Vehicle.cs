using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrailEntities.Entity
{
    /// <summary>
    ///     Vessel that holds all the players, their inventory, money, and keeps track of total miles traveled in the form of
    ///     an odometer.
    /// </summary>
    public sealed class Vehicle : IEntity
    {
        /// <summary>
        ///     Fired when the user changes the pace of traveling.
        /// </summary>
        public delegate void OnChangePace();

        /// <summary>
        ///     Fired when the user changes the ration for group.
        /// </summary>
        public delegate void OnChangeRation();

        /// <summary>
        ///     References the vehicle itself, it is important to remember the vehicle is not an entity and not an item.
        /// </summary>
        private Dictionary<SimEntity, SimItem> _inventory;

        /// <summary>
        ///     References all of the people inside of the vehicle.
        /// </summary>
        private List<Person> _passengers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Entity.Vehicle" /> class.
        /// </summary>
        public Vehicle()
        {
            ResetVehicle(0);
            Name = "Vehicle";
            Pace = TravelPace.Steady;
            Mileage = 1;
        }

        /// <summary>
        ///     References the vehicle itself, it is important to remember the vehicle is not an entity and not an item.
        /// </summary>
        public IDictionary<SimEntity, SimItem> Inventory
        {
            get { return _inventory; }
        }

        /// <summary>
        ///     References all of the people inside of the vehicle.
        /// </summary>
        public IEnumerable<Person> Passengers
        {
            get { return _passengers; }
        }

        /// <summary>
        ///     Current ration level, determines the amount food that will be consumed each day of the simulation.
        /// </summary>
        public RationLevel Ration { get; private set; }

        /// <summary>
        ///     Current travel pace, determines how fast the vehicle will attempt to move down the trail.
        /// </summary>
        public TravelPace Pace { get; private set; }

        /// <summary>
        ///     Current health of the vehicle, determines how well it will be able to perform
        /// </summary>
        public RepairStatus RepairStatus { get; private set; }

        /// <summary>
        ///     Total number of miles the vehicle has traveled since the start of the simulation.
        /// </summary>
        public int Odometer { get; private set; }

        /// <summary>
        ///     In general, you will travel 200 miles plus some additional distance which depends upon the quality of your team of
        ///     oxen. This mileage figure is an ideal, assuming nothing goes wrong. If you run into problems, mileage is subtracted
        ///     from this ideal figure; the revised total is printed at the start of the next trip segment.
        /// </summary>
        public int Mileage { get; private set; }

        /// <summary>
        ///     Returns the total value of all the cash the vehicle and all party members currently have.
        ///     Setting this value will change the quantity of dollar bills in player inventory.
        /// </summary>
        public float Balance
        {
            get { return _inventory[SimEntity.Cash].TotalValue; }
            private set
            {
                // Skip if the quantity already matches the value we are going to set it to.
                if (value.Equals(_inventory[SimEntity.Cash].Quantity))
                    return;

                // Check if the value being set is zero, if so just reset it.
                if (value <= 0)
                {
                    _inventory[SimEntity.Cash].Reset();
                }
                else
                {
                    _inventory[SimEntity.Cash] = new SimItem(_inventory[SimEntity.Cash], (int) value);
                }
            }
        }

        /// <summary>
        ///     Name of the entity as it should be known in the simulation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Defines what type of entity this will take the role of in the simulation. Depending on this value the simulation
        ///     will affect how it is treated, points tabulated, and interactions governed.
        /// </summary>
        public SimEntity Category
        {
            get { return SimEntity.Vehicle; }
        }

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

        /// <summary>
        ///     Reduces the total mileage the vehicle has rolled to move within the next two week block section. Will not allow
        ///     mileage to be reduced below zero.
        /// </summary>
        /// <param name="amount">Amount of mileage that will be reduced.</param>
        internal void ReduceMileage(int amount)
        {
            // Check if current mileage is below zero.
            if (Mileage <= 0)
                return;

            // Calculate new mileage.
            var updatedMileage = Mileage - amount;

            // Check if updated mileage is below zero.
            if (updatedMileage <= 0)
                updatedMileage = 0;

            // Check that mileage doesn't already exist as this value somehow.
            if (!updatedMileage.Equals(Mileage))
            {
                // Set mileage to new updated value.
                Mileage = updatedMileage;
            }
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
        public void BuyItem(SimItem transaction)
        {
            // Check of the player can afford this item.
            if (Balance <= transaction.TotalValue)
                return;

            // Create new item based on old one, with new quantity value from store, trader, random event, etc.
            Balance -= transaction.TotalValue;
            _inventory[transaction.Category] = new SimItem(_inventory[transaction.Category], transaction.Quantity);
        }

        /// <summary>
        ///     Resets the vehicle status to the defaults.
        /// </summary>
        /// <param name="startingMonies">Amount of money the vehicle should have to work with.</param>
        public void ResetVehicle(int startingMonies)
        {
            _inventory = new Dictionary<SimEntity, SimItem>(GameSimApp.DefaultInventory);
            Balance = startingMonies;
            _passengers = new List<Person>();
            Ration = RationLevel.Filling;
            RepairStatus = RepairStatus.Good;
            Odometer = 0;
        }

        /// <summary>
        ///     Processes logic and events for vehicle, also progresses down the trail and keeps track of mileage for this turn.
        /// </summary>
        public void TickVehicle()
        {
            // Figure out how far we need to go to reach the next point.
            Mileage = GameSimApp.Instance.Trail.DistanceToNextLocation;

            // Determine how many miles we can move in a day on the trail based on amount of monies player spent on oxen to pull vehicle.
            var cost_animals = GameSimApp.Instance.Vehicle.Inventory[SimEntity.Animal].TotalValue;
            Mileage = (int) (Mileage + 200 + (cost_animals - 220)/5 + 10*GameSimApp.Instance.Random.NextDouble());

            // Sometimes things just go slow on the trail.
            Mileage = (int) (Mileage - 45 - GameSimApp.Instance.Random.NextDouble()/.02f);

            // Determines how much food party members in the vehicle will eat today.
            var cost_food = Inventory[SimEntity.Food].TotalValue;
            var two_weeks_fraction = (GameSimApp.TRAIL_LENGTH - Odometer)/(Mileage - Odometer);
            cost_food = cost_food - 8 - 5*(int) Ration;
            if (cost_food >= 13)
            {
                cost_food = cost_food + (1 - two_weeks_fraction)*(8 + 5*(int) Ration);
                //cost_food = cost_food + 8 + 5*(int) Ration;
                Inventory[SimEntity.Food] = new SimItem(Inventory[SimEntity.Food], (int) cost_food);
            }

            // TODO: Determine if weather will slow us down.


            // Loop through all the people in the vehicle and tick them.
            foreach (var person in _passengers)
            {
                person.TickPerson();
            }

            // Check for random events that might trigger.
            GameSimApp.Instance.Director.TriggerRandomEvent();

            // Use our altered mileage to affect how far the vehicle has traveled in todays tick..
            Odometer += Mileage;
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