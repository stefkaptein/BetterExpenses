using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Models.Tasks;


[Index(nameof(Id))]
[Index(nameof(CreationDate))]
public abstract class CalculatorTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}