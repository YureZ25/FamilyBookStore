using Data.Entities.Contracts;

namespace Data.Entities
{
    public class Book2Stores : IEntity
    {
        public int BookId { get; set; }

        public int StoreId { get; set; }
    }
}
