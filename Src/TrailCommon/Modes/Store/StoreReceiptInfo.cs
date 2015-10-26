using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreReceiptInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private List<StoreTransactionItem> _totalTransactions;

        /// <summary>
        ///     Creates a new store transaction tracker.
        /// </summary>
        public StoreReceiptInfo()
        {
            _totalTransactions = new List<StoreTransactionItem>();
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public ReadOnlyCollection<StoreTransactionItem> Transactions
        {
            get { return _totalTransactions.AsReadOnly(); }
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
        private void RemoveItem(Item item)
        {
            // Loop through every single transaction.
            var copyList = new List<StoreTransactionItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if item name matches incoming one.
                if (!transaction.Item.Name.Equals(item.Name))
                    continue;

                // Remove that tuple from transaction list.
                _totalTransactions.Remove(transaction);
                break;
            }
        }
    }
}