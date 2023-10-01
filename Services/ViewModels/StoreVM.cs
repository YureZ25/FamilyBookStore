namespace Services.ViewModels
{
    public class StoreVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BookVM> Books { get; set; }
    }
}
