using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CashFlo.Abstraction;
using CashFlo.Model;
using CashFlo.Services.Interface;

namespace CashFlo.Services
{
    public class TransactionServices : UserBase, ITransactionService
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "transactions.json");

        // Load transactions from the JSON file
        public async Task<List<TransactionM>> GetAllTransactionsAsync(string username)
        {
            if (!File.Exists(FilePath)) return new List<TransactionM>();

            try
            {
                var json = await File.ReadAllTextAsync(FilePath);
                var transactions = JsonSerializer.Deserialize<List<TransactionM>>(json) ?? new List<TransactionM>();
                return transactions.Where(t => t.Username == username).OrderByDescending(t => t.Date).ToList();
            }
            catch (Exception)
            {
                // Log or handle the exception
                return new List<TransactionM>(); // Return an empty list on failure
            }
        }

        // Search for transactions with filter and search text
        public async Task<List<TransactionM>> SearchTransactionsAsync(string username, string searchText)
        {
            var allTransactions = await GetAllTransactionsAsync(username);
            IEnumerable<TransactionM> filteredTransactions = allTransactions;

            // Filter by search text
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredTransactions = filteredTransactions.Where(t =>
                    t.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (t.Tags != null && t.Tags.Any(tag => tag.Contains(searchText, StringComparison.OrdinalIgnoreCase))));
            }

            

            return filteredTransactions.ToList();
        }

        // Add a new transaction
        public async Task<bool> AddTransactionAsync(TransactionM transaction)
        {
            try
            {
                var transactions = await GetAllTransactionsAsync(transaction.Username);
                transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1; // Generate a new ID
                transactions.Add(transaction);

                await SaveTransactionsAsync(transactions);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("Failed to add transaction", ex);
            }
        }

        // Update an existing transaction
        public async Task<bool> UpdateTransactionAsync(TransactionM updatedTransaction)
        {
            try
            {
                var transactions = await GetAllTransactionsAsync(updatedTransaction.Username);
                var existingTransaction = transactions.FirstOrDefault(t => t.Id == updatedTransaction.Id);

                if (existingTransaction != null)
                {
                    existingTransaction.Amount = updatedTransaction.Amount;
                    existingTransaction.Date = updatedTransaction.Date;
                    existingTransaction.Category = updatedTransaction.Category;
                    existingTransaction.Description = updatedTransaction.Description;
                    existingTransaction.Tags = updatedTransaction.Tags;

                    await SaveTransactionsAsync(transactions);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("Failed to update transaction", ex);
            }
        }

        // Delete a transaction
        public async Task<bool> DeleteTransactionAsync(int transactionId, string username)
        {
            try
            {
                var transactions = await GetAllTransactionsAsync(username);
                var transactionToDelete = transactions.FirstOrDefault(t => t.Id == transactionId);

                if (transactionToDelete != null)
                {
                    transactions.Remove(transactionToDelete);
                    await SaveTransactionsAsync(transactions);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("Failed to delete transaction", ex);
            }
        }

        // Save the list of transactions to the JSON file
        private async Task SaveTransactionsAsync(List<TransactionM> transactions)
        {
            try
            {
                var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("Failed to save transactions", ex);
            }
        }
    }
}
