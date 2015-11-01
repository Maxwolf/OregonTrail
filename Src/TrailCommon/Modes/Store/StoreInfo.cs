using System.Collections.Generic;

namespace TrailCommon
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreInfo : ModeInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private HashSet<StoreTransactionItem> _totalTransactions;

        /// <summary>
        ///     Creates a new store transaction tracker.
        /// </summary>
        /// <param name="showAdvice">Sets the bool for showing store advice, defaults to false.</param>
        public StoreInfo(bool showAdvice = false)
        {
            ShowStoreAdvice = showAdvice;
            _totalTransactions = new HashSet<StoreTransactionItem>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            ShowStoreAdvice = false;
            _totalTransactions = new HashSet<StoreTransactionItem>();
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IEnumerable<StoreTransactionItem> Transactions
        {
            get { return _totalTransactions; }
        }

        protected override string Name
        {
            get { return "Store Receipt Information"; }
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
        public uint GetTransactionTotalCost()
        {
            // Loop through all transactions and multiply amount by cost.
            float totalCost = 0;
            foreach (var tuple in _totalTransactions)
            {
                totalCost += tuple.Quantity*tuple.Item.Cost;
            }

            // Cast to unsigned integer and return.
            return (uint) totalCost;
        }

        /// <summary>
        ///     Adds an item to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(Item item, uint amount)
        {
            // Create the tuple for the item to add.
            var incomingTuple = new StoreTransactionItem(amount, item);

            // Remove any existing tuple with this item name, we will replace it.
            RemoveItem(item);

            // Add the new tuple to replace the one we just removed.
            _totalTransactions.Add(incomingTuple);
        }

        /// <summary>
        ///     Removes an item from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        private void RemoveItem(IEntity item)
        {
            // Loop through every single transaction.
            var copyList = new HashSet<StoreTransactionItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if item name matches incoming one.
                if (!transaction.Item.Equals(item))
                    continue;

                // Remove that tuple from transaction list.
                _totalTransactions.Remove(transaction);
                break;
            }
        }
    }
}