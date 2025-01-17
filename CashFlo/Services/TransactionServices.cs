using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CashFlo.Model;
using CashFlo.Services.Interface;

namespace CashFlo.Services
{
    public class TransactionServices : ITransactionService
    {
        protected static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");

        // Load transactions from the JSON file
        private List<TransactionM> LoadTransactions()
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<TransactionM>>(json) ?? new List<TransactionM>();
            }
            catch
            {
                return new List<TransactionM>(); // Return an empty list on failure
            }
        }

        // Save transactions to the JSON file
        private void SaveTransactions(List<TransactionM> transactions)
        {
            try
            {
                var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save transactions", ex);
            }
        }

        // Get all transactions for a user
        public List<TransactionM> GetAllTransactions(string username)
        {
            var transactions = LoadTransactions();
            return transactions.Where(t => t.Username == username).OrderByDescending(t => t.Date).ToList();
        }

        // Search transactions for a user
        public List<TransactionM> SearchTransactions(string username, string searchText)
        {
            var transactions = GetAllTransactions(username);
            return transactions.Where(t =>
                string.IsNullOrEmpty(searchText) ||
                t.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (t.Tags != null && t.Tags.Any(tag => tag.Contains(searchText, StringComparison.OrdinalIgnoreCase)))).ToList();
        }

        // Add a new transaction
        public bool AddTransaction(TransactionM transaction)
        {
            var transactions = LoadTransactions();
            transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1; 
            transactions.Add(transaction);
            SaveTransactions(transactions);
            return true;
        }        

        // Delete a transaction
        public bool DeleteTransaction(int transactionId, string username)
        {
            var transactions = GetAllTransactions(username);
            var transactionToDelete = transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transactionToDelete != null)
            {
                transactions.Remove(transactionToDelete);
                SaveTransactions(transactions);
                return true;
            }
            return false;
        }

        // Get total balance for a user
        public decimal GetUserTotalBalance(string username)
        {
            var transactions = GetAllTransactions(username);
            return transactions.Sum(t => t.Amount);
        }

        // Save transaction (Add or Update)
        public void SaveTransactions(TransactionM transaction)
        {
            var transactions = LoadTransactions();
            var existingTransaction = transactions.FirstOrDefault(t => t.Id == transaction.Id);

            if (existingTransaction != null)
            {
                // Update the existing transaction
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Date = transaction.Date;
                existingTransaction.Category = transaction.Category;
                existingTransaction.Description = transaction.Description;
                existingTransaction.Tags = transaction.Tags;
            }
            else
            {
                // Add the new transaction
                transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;
                transactions.Add(transaction);
            }

            // Save the updated list of transactions
            SaveTransactions(transactions);
        }
        // Get the total number of transactions for a user
        public int GetTotalTransactionCount(string username)
        {
            var transactions = GetAllTransactions(username);
            return transactions.Count();
        }

        
        public decimal GetTotalTransactionValue(string username)
        {
            var transactions = GetAllTransactions(username);
            decimal totalAmount = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Type == "Income" )  
                {
                    totalAmount += transaction.Amount;
                }
                else if (transaction.Type == "Expenses")  
                {
                    totalAmount -= transaction.Amount;
                }
            }

            return totalAmount;
        }

    }
}
