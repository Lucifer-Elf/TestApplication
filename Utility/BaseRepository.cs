namespace Servize.Utility
{
    public abstract class BaseRepository<T>
    {
        private readonly ServizeDBContext _context;

        public BaseRepository(ServizeDBContext context)
        {
            _context = context;
        }

        public ServizeDBContext GetContext()
        {
            return _context;
        }

    }
}
