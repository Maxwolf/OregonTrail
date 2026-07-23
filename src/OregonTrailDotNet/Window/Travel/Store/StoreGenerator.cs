// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Vehicle;

namespace OregonTrailDotNet.Window.Travel.Store
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreGenerator
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<EntitiesEnum, SimItem> _totalTransactions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoreGenerator" /> class.
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public StoreGenerator()
        {
            Reset();
        }

        /// <summary>
        ///     Item which the player does not have enough of or is missing.
        /// </summary>
        public SimItem SelectedItem { get; set; }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<EntitiesEnum, SimItem> Transactions => _totalTransactions;

        /// <summary>
        ///     Returns the total cost of all the transactions this receipt information object represents.
        /// </summary>
        public float TotalTransactionCost
        {
            get
            {
                // Loop through all transactions and multiply amount by cost.
                float totalCost = 0;
                foreach (var item in _totalTransactions)
                    totalCost += item.Value.Quantity*item.Value.Cost;

                // Cast to unsigned integer and return.
                return totalCost;
            }
        }

        /// <summary>
        ///     Minimum number of oxen the party must buy before leaving Matt's General Store: a single yoke of two, the same
        ///     forced $40 spend the 1985 game required to start the trip (which is what pins the farmer's best possible
        ///     leftover cash at $360 and the maximum score at 13,860).
        /// </summary>
        internal const int MinimumOxen = 2;

        /// <summary>
        ///     Checks if the player has enough animals to pull their vehicle.
        /// </summary>
        /// <returns>TRUE if player is missing enough items to correctly start the game, FALSE if everything is OK.</returns>
        internal bool MissingImportantItems => GameSimulationApp.Instance.Trail.IsFirstLocation &&
                                               (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Unreached) &&
                                               (_totalTransactions[EntitiesEnum.Animal].Quantity < MinimumOxen);

        /// <summary>
        ///     Processes all of the pending transactions in the store receipt info object.
        /// </summary>
        internal void PurchaseItems()
        {
            // Attempt every pending transaction. Vehicle.Purchase already refuses (silently skips) any single item the
            // player cannot afford, so the wagon balance can never be driven negative here. This deliberately does NOT
            // throw when the running total exceeds the balance: the store reaches this method from the on-trail
            // immediate-buy path with no debt-warning gate in front of it, and a hard exception there crashed the game.
            // Affordability is enforced up front in StorePurchase (per item, accounting for each item's minimum lot) and,
            // on the first location, by the StoreDebtWarning check before checkout.
            foreach (var transaction in _totalTransactions)
                GameSimulationApp.Instance.Vehicle.Purchase(transaction.Value);

            // Remove all the transactions now that we have processed them.
            Reset();
        }

        /// <summary>
        ///     Cleans out all the transactions, if they have not been processed yet then they will be lost forever.
        /// </summary>
        private void Reset()
        {
            _totalTransactions = new Dictionary<EntitiesEnum, SimItem>(Vehicle.DefaultInventory);
        }

        /// <summary>Adds an SimItem to the list of pending transactions. If it already exists it will be replaced.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">The amount.</param>
        public void AddItem(SimItem item, int amount)
        {
            _totalTransactions[item.Category] = new SimItem(item, amount);
        }

        /// <summary>Removes an SimItem from the list of pending transactions. If it does not exist then nothing will happen.</summary>
        /// <param name="item">The item.</param>
        public void RemoveItem(SimItem item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<EntitiesEnum, SimItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if SimItem name matches incoming one.
                if (!transaction.Key.Equals(item.Category))
                    continue;

                // Reset the simulation SimItem to default values, meaning the player has none of them.
                _totalTransactions[item.Category].Reset();
                break;
            }
        }
    }
}