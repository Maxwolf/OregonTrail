using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<SimEntity, SimItem> _totalTransactions;

        /// <summary>
        ///     Creates a new store transaction tracker.
        /// </summary>
        /// <param name="showAdvice">Sets the bool for showing store advice, defaults to false.</param>
        public StoreInfo(bool showAdvice = false)
        {
            ShowStoreAdvice = showAdvice;
            _totalTransactions = new Dictionary<SimEntity, SimItem>(Resources.DefaultStore);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            ShowStoreAdvice = false;
            _totalTransactions = new Dictionary<SimEntity, SimItem>();
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<SimEntity, SimItem> Transactions
        {
            get { return _totalTransactions; }
        }

        /// <summary>
        ///     Determines if we have already shown the advice to the player.
        /// </summary>
        public bool ShowStoreAdvice { get; set; }

        public void ClearTransactions()
        {
            _totalTransactions.Clear();
        }

        /// <summary>
        ///     Returns the total cost of all the transactions this receipt information object represents.
        /// </summary>
        public float GetTransactionTotalCost()
        {
            // Loop through all transactions and multiply amount by cost.
            float totalCost = 0;
            foreach (var item in _totalTransactions)
            {
                totalCost += item.Value.Quantity*item.Value.Cost;
            }

            // Cast to unsigned integer and return.
            return totalCost;
        }

        /// <summary>
        ///     Adds an SimItem to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(SimItem simItem, int amount)
        {
            // Create the tuple for the SimItem to add.
            var incomingPurchase = new SimItem(simItem, amount);

            // Remove any existing tuple with this SimItem name, we will replace it.
            RemoveItem(simItem);

            // Add the new tuple to replace the one we just removed.
            _totalTransactions.Add(incomingPurchase.Category, incomingPurchase);
        }

        /// <summary>
        ///     Removes an SimItem from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(IEntity item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<SimEntity, SimItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if SimItem name matches incoming one.
                if (!transaction.Key.Equals(item.Category))
                    continue;

                // Remove that SimItem from transaction list.
                _totalTransactions.Remove(item.Category);
                break;
            }
        }
    }
}