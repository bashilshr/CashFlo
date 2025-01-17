using System.Collections.Generic;
using CashFlo.Model;

namespace CashFlo.Services.Interface
{
    public interface ITransactionService
    {
        // Method to get all transactions for a specific user
        List<TransactionM> GetAllTransactions(string username);

        // Method to search for transactions based on search text
        List<TransactionM> SearchTransactions(string username, string searchText);

        // Method to add a new transaction
        bool AddTransaction(TransactionM transaction);

        // Method to update an existing transaction
        // Method to delete a transaction by its ID and username
        bool DeleteTransaction(int transactionId, string username);

        // Method to save the list of transactions to the JSON file
        void SaveTransactions(TransactionM transaction);

        // Method to get the total balance for a user
        decimal GetUserTotalBalance(string username);
    }
}
