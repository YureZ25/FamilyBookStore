using Data.Entities.Contracts;

namespace Data.Entities
{
    public class Store : ICommonEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public IEnumerable<Book> Books { get; set; }
    }
}
