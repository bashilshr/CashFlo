using CashFlo.Model;
using CashFlo.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CashFlo.Services
{
    public class DebtServices : IDebtServices
    {
        // Set the file path to a writable location in the user's AppData folder
        protected static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "debt.json");

        private List<Debt> debts;

        public void AddDebt(Debt debt)
        {
            debt.Id = Guid.NewGuid();  // Generate unique Id for new debt
            debts.Add(debt);
            SaveDebts();
        }

        public bool MarkDebtAsPaid(Guid debtId)
        {
            var debt = debts.FirstOrDefault(d => d.Id == debtId);
            if (debt == null)
                return false;

            debt.Status = DebtStatus.Paid;  // Correcting the enum value
            SaveDebts();
            return true;
        }

        public bool DeleteDebt(Guid debtId)
        {
            var debt = debts.FirstOrDefault(d => d.Id == debtId);
            if (debt == null)
                return false;

            debts.Remove(debt);
            SaveDebts();
            return true;
        }

        public List<Debt> GetAllDebts()
        {
            return debts;
        }

        public List<Debt> GetDebtsByLender(string lenderName)
        {
            return debts.Where(d => d.Lender.Contains(lenderName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void SaveDebts()
        {
            // Ensure the folder exists before writing the file
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);  // Create the folder if it doesn't exist
            }

            var json = JsonConvert.SerializeObject(debts, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public void LoadDebts()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                debts = JsonConvert.DeserializeObject<List<Debt>>(json) ?? new List<Debt>();
            }
            else
            {
                debts = new List<Debt>();
            }
        }

        public void UpdateDebt(Debt debt)
        {
            var existingDebt = debts.FirstOrDefault(d => d.Id == debt.Id);
            if (existingDebt != null)
            {
                existingDebt.Lender = debt.Lender;
                existingDebt.Amount = debt.Amount;
                existingDebt.DueDate = debt.DueDate;
                existingDebt.Status = debt.Status;
                existingDebt.Notes = debt.Notes;
                SaveDebts();
            }
        }

        public List<Debt> GetDebtsByStatus(DebtStatus status)
        {
            return debts.Where(d => d.Status == status).ToList();  // Returning debts based on the given status
        }
        
    }


}
