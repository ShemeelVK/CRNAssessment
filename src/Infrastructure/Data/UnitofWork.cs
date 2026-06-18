
using CRNAssessment.Infrastructure.Data;

namespace CRNAssessment.Infrastructure.Data;

public class UnitOfWork
{
    private readonly ApplicationDbContext _context;
    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
}
