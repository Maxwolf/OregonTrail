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
        private HashSet<Item> _totalTransactions;

        /// <summary>
        ///     Creates a new store transaction tracker.
        /// </summary>
        /// <param name="showAdvice">Sets the bool for showing store advice, defaults to false.</param>
        public StoreInfo(bool showAdvice = false)
        {
            ShowStoreAdvice = showAdvice;
            _totalTransactions = new HashSet<Item>();

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            ShowStoreAdvice = false;
            _totalTransactions = new HashSet<Item>();
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IEnumerable<Item> Transactions
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
                totalCost += item.Quantity*item.Cost;
            }

            // Cast to unsigned integer and return.
            return totalCost;
        }

        /// <summary>
        ///     Adds an item to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(Item item, int amount)
        {
            // Create the tuple for the item to add.
            var incomingPurchase = new Item(item, amount);

            // Remove any existing tuple with this item name, we will replace it.
            RemoveItem(item);

            // Add the new tuple to replace the one we just removed.
            _totalTransactions.Add(incomingPurchase);
        }

        /// <summary>
        ///     Removes an item from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(IEntity item)
        {
            // Loop through every single transaction.
            var copyList = new HashSet<Item>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if item name matches incoming one.
                if (!transaction.Equals(item))
                    continue;

                // Remove that tuple from transaction list.
                _totalTransactions.Remove(transaction);
                break;
            }
        }
    }
}