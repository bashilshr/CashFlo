using System;
using System.Collections.Generic;

namespace CashFlo.Model
{
    public class TransactionM
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } // Income, Expense, etc.
        public TransactionType Type { get; set; } // Enum for Income/Expense
        public List<string> Tags { get; set; } = new List<string>(); // Tags for the transaction

        
    }
}
