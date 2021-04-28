using System.Threading.Tasks;

namespace Servize.Utility
{
    public class ContextTransaction
    {
        private readonly ServizeDBContext _context;
        public ContextTransaction(ServizeDBContext context)
        {
            _context = context;
        }
        public async Task CompleteAsync()
        {
            _context.UpdateModifiedField();
            await _context.SaveChangesAsync(true);
        }
    }
}
