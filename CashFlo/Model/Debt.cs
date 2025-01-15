namespace CashFlo.Model
{
    public class Debt
    {
        public Guid Id { get; set; }
        public string Lender { get; set; }  // Who the debt is owed to
        public decimal Amount { get; set; }  // Amount of debt
        public DateTime DueDate { get; set; }  // When the debt is due
        public String Status { get; set; } // Whether the debt is paid or not
        public string Notes { get; set; }  // Any additional notes
    }
}
