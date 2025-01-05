using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlo.Model;

namespace CashFlo.Services.Interface
{
    public interface ITransactionService
    {
        // Method to get all transactions for a specific user
        Task<List<TransactionM>> GetAllTransactionsAsync(string username);

        // Method to search for transactions based on search text
        Task<List<TransactionM>> SearchTransactionsAsync(string username, string searchText);

        // Method to add a new transaction
        Task<bool> AddTransactionAsync(TransactionM transaction);

        // Method to update an existing transaction
        Task<bool> UpdateTransactionAsync(TransactionM updatedTransaction);

        // Method to delete a transaction by its ID and username
        Task<bool> DeleteTransactionAsync(int transactionId, string username);

        // Method to save the list of transactions to the JSON file
        //Task SaveTransactionsAsync(TransactionM transactions);
    }
}
