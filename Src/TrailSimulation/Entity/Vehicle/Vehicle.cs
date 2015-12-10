using System;
using System.Collections.Generic;
using System.Diagnostics;
using TrailSimulation.Event;
using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Vessel that holds all the players, their inventory, money, and keeps track of total miles traveled in the form of
    ///     an odometer.
    /// </summary>
    public sealed class Vehicle : IEntity
    {
        /// <summary>
        ///     References the vehicle itself, it is important to remember the vehicle is not an entity and not an item.
        /// </summary>
        private Dictionary<Entities, SimItem> _inventory;

        /// <summary>
        ///     References all of the people inside of the vehicle.
        /// </summary>
        private List<Person> _passengers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Entities.Vehicle" /> class.
        /// </summary>
        public Vehicle()
        {
            ResetVehicle(0);
            Name = "Vehicle";
            Pace = TravelPace.Steady;
            Mileage = 1;
            Status = VehicleStatus.Stopped;
        }

        /// <summary>
        ///     References the vehicle itself, it is important to remember the vehicle is not an entity and not an item.
        /// </summary>
        public IDictionary<Entities, SimItem> Inventory
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
        public Health Health { get; private set; }

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
        ///     Defines what the trail module is currently processing if anything in regards to movement of vehicle and player
        ///     entities down the trail.
        /// </summary>
        public VehicleStatus Status { get; set; }

        /// <summary>
        ///     Returns the total value of all the cash the vehicle and all party members currently have.
        ///     Setting this value will change the quantity of dollar bills in player inventory.
        /// </summary>
        public float Balance
        {
            get { return _inventory[Entities.Cash].TotalValue; }
            private set
            {
                // Skip if the quantity already matches the value we are going to set it to.
                if (value.Equals(_inventory[Entities.Cash].Quantity))
                    return;

                // Check if the value being set is zero, if so just reset it.
                if (value <= 0)
                {
                    _inventory[Entities.Cash].Reset();
                }
                else
                {
                    _inventory[Entities.Cash] = new SimItem(_inventory[Entities.Cash],
                        (int) value);
                }
            }
        }

        /// <summary>
        ///     Default items every vehicle and store will have, their prices increase with distance from starting point.
        /// </summary>
        internal static IDictionary<Entities, SimItem> DefaultInventory
        {
            get
            {
                var defaultInventory = new Dictionary<Entities, SimItem>
                {
                    {Entities.Animal, Parts.Oxen},
                    {Entities.Clothes, Resources.Clothing},
                    {Entities.Ammo, Resources.Bullets},
                    {Entities.Wheel, Parts.Wheel},
                    {Entities.Axle, Parts.Axle},
                    {Entities.Tongue, Parts.Tongue},
                    {Entities.Food, Resources.Food},
                    {Entities.Cash, Resources.Cash}
                };
                return defaultInventory;
            }
        }

        /// <summary>
        ///     In general, you will travel 200 miles plus some additional distance which depends upon the quality of your team of
        ///     oxen. This mileage figure is an ideal, assuming nothing goes wrong. If you run into problems, mileage is subtracted
        ///     from this ideal figure; the revised total is printed at the start of the next trip segment.
        /// </summary>
        /// <returns>The expected mileage over the next two week segment.</returns>
        private int RandomMileage
        {
            get
            {
                // Total amount of monies the player has spent on animals to pull their vehicle.
                var cost_animals = Inventory[Entities.Animal].TotalValue;

                // Variables that will hold the distance we should travel in the next day.
                var total_miles = Mileage + (cost_animals - 110)/2.5 + 10*GameSimulationApp.Instance.Random.NextDouble();

                return (int) Math.Abs(total_miles);
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
        public Entities Category
        {
            get { return Entities.Vehicle; }
        }

        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

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

        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        public void OnTick(bool systemTick)
        {
            // Only can tick vehicle on interval.
            if (systemTick)
                return;

            // Loop through all the people in the vehicle and tick them every day of simulation moving or not.
            foreach (var person in _passengers)
                person.OnTick(false);

            // Only advance the vehicle mileage and odometer if we are actually traveling.
            if (Status != VehicleStatus.Moving)
                return;

            // Figure out how far we need to go to reach the next point.
            Mileage = RandomMileage;

            // Sometimes things just go slow on the trail, cut mileage in half if above zero randomly.
            if (GameSimulationApp.Instance.Random.NextBool() && Mileage > 0)
                Mileage = Mileage/2;

            // Check for random events that might trigger regardless of calculations made.
            GameSimulationApp.Instance.EventDirector.TriggerEventByType(this, EventCategory.Vehicle);

            // Check to make sure mileage is never below or at zero.
            if (Mileage <= 0)
                Mileage = 10;

            // Use our altered mileage to affect how far the vehicle has traveled in todays tick..
            Odometer += Mileage;
        }

        /// <summary>
        ///     Reduces the total mileage the vehicle has rolled to move within the next two week block section. Will not allow
        ///     mileage to be reduced below zero.
        /// </summary>
        /// <param name="amount">Amount of mileage that will be reduced.</param>
        internal void ReduceMileage(int amount)
        {
            // Mileage cannot be reduced when parked.
            if (Status != VehicleStatus.Moving)
                return;

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

        /// <summary>
        ///     Sets the current speed of the game simulation.
        /// </summary>
        public void ChangePace(TravelPace castedSpeed)
        {
            // Change game simulation speed.
            Pace = castedSpeed;
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
            if (Balance < transaction.TotalValue)
                return;

            // Create new item based on old one, with new quantity value from store, trader, random event, etc.
            Balance -= transaction.TotalValue;

            // Make sure we add the quantity and not just replace it.
            var oldQuantity = _inventory[transaction.Category].Quantity;
            _inventory[transaction.Category] = new SimItem(_inventory[transaction.Category],
                oldQuantity + transaction.Quantity);
        }

        /// <summary>
        ///     Resets the vehicle status to the defaults.
        /// </summary>
        /// <param name="startingMonies">Amount of money the vehicle should have to work with.</param>
        public void ResetVehicle(int startingMonies)
        {
            _inventory = new Dictionary<Entities, SimItem>(DefaultInventory);
            Balance = startingMonies;
            _passengers = new List<Person>();
            Ration = RationLevel.Filling;
            Health = Health.Good;
            Odometer = 0;
            Status = VehicleStatus.Stopped;
        }

        /// <summary>
        ///     Changes the current ration level to new value if it is not already set to that. Also fires even about this for
        ///     subscribers to get event notification about the change.
        /// </summary>
        /// <param name="ration">The rate at which people are permitted to eat in the vehicle party.</param>
        public void ChangeRations(RationLevel ration)
        {
            Ration = ration;
        }

        /// <summary>
        ///     Destroys some of the inventory items in no particular order and or reason. That is left up the caller to decide.
        /// </summary>
        public IDictionary<Entities, int> DestroyRandomItems()
        {
            // Dictionary that will keep track of enumeration item type and destroyed amount for record keeping purposes.
            IDictionary<Entities, int> destroyedItems = new Dictionary<Entities, int>();

            // Make a copy of the inventory to iterate over.
            var copiedInventory = new Dictionary<Entities, SimItem>(Inventory);

            // Loop through the inventory and decide to randomly destroy some inventory items.
            foreach (var itemPair in copiedInventory)
            {
                // Skip item if quantity is less than one.
                if (itemPair.Value.Quantity < 1)
                    continue;

                // Determine if we will be destroying this item.
                if (GameSimulationApp.Instance.Random.NextBool())
                    continue;

                // Destroy some random amount of the item from one to total amount.
                var destroyAmount = GameSimulationApp.Instance.Random.Next(1, itemPair.Value.Quantity);

                // Subtract the amount we destroyed from the actual inventory.
                Inventory[itemPair.Key] = new SimItem(itemPair.Value, itemPair.Value.Quantity - destroyAmount);

                // Tabulate the amount we destroyed in dictionary to be returned to caller.
                destroyedItems.Add(itemPair.Key, destroyAmount);
            }

            // Clear out the copied list we made for iterating.
            copiedInventory.Clear();

            // Return the destroyed item summary.
            return destroyedItems;
        }
    }
}