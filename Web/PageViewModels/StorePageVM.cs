using Services.ViewModels.StoreVMs;

namespace Web.PageViewModels
{
    public class StorePageVM
    {
        public StoreGetVM StoreGet { get; set; }
        public StorePostVM StorePost { get; set; }

        public StorePageVM()
        {
            
        }

        public StorePageVM(StoreGetVM storeGet)
        {
            StoreGet = storeGet;
        }

        public StorePageVM(StorePostVM storePost)
        {
            StoreGet = new StoreGetVM
            {
                Id = storePost.Id ?? 0,
                Name = storePost.Name,
                Address = storePost.Address,
            };
        }
    }
}
