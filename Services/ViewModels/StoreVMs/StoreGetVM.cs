using Services.ViewModels.BookVMs;

namespace Services.ViewModels.StoreVMs
{
    public class StoreGetVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public bool IsLinkedToUser { get; init; }

        public IEnumerable<BookGetVM> Books { get; set; }
    }
}
