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
        private List<IItem> _transactions;

        /// <summary>
        ///     Creates a new store transaction tracker.
        /// </summary>
        public StoreReceiptInfo()
        {
            _transactions = new List<IItem>();
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public ReadOnlyCollection<IItem> Transactions
        {
            get { return _transactions.AsReadOnly(); }
        }

        /// <summary>
        ///     Adds an item to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(IItem item)
        {
            if (!_transactions.Contains(item))
                _transactions.Remove(item);

            _transactions.Add(item);
        }

        /// <summary>
        ///     Removes an item from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(IItem item)
        {
            if (_transactions.Contains(item))
                _transactions.Remove(item);
        }
    }
}