using CashFlo.Model;
using System;
using System.Collections.Generic;

namespace CashFlo.Services.Interface
{
    public interface IDebtServices
    {
        void AddDebt(Debt debt);
        bool MarkDebtAsPaid(Guid debtId);
        bool DeleteDebt(Guid debtId);
        void UpdateDebt(Debt debt);
        List<Debt> GetAllDebts();
        List<Debt> GetDebtsByStatus(DebtStatus status);
        List<Debt> GetDebtsByLender(string lenderName);
        void SaveDebts();
        void LoadDebts();

    }

}

