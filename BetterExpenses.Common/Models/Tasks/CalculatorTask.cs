using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Models.Tasks;


[Index(nameof(Id))]
[Index(nameof(CreationDate), nameof(Priority))]
[Index(nameof(Status))]
public abstract class CalculatorTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int Priority { get; set; } = 1;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime ProcessDate { get; set; }
    public DateTime FinishDate { get; set; }
    public CalculatorTaskStatus Status { get; set; } = CalculatorTaskStatus.Pending;
}