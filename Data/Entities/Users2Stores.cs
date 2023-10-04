using Data.Entities.Contracts;

namespace Data.Entities
{
    public class Users2Stores : IEntity
    {
        public int UserId { get; set; }

        public int StoreId { get; set; }
    }
}
