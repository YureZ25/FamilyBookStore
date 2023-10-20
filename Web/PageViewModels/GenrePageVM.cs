using Services.ViewModels.GenreVMs;

namespace Web.PageViewModels
{
    public class GenrePageVM
    {
        public GenreGetVM GenreGet { get; set; }
        public GenrePostVM GenrePost { get; set; }

        public GenrePageVM()
        {
            
        }

        public GenrePageVM(GenreGetVM genreGet)
        {
            GenreGet = genreGet;
        }

        public GenrePageVM(GenrePostVM genrePost)
        {
            GenreGet = new GenreGetVM
            {
                Id = genrePost.Id ?? 0,
                Name = genrePost.Name,
            };
        }
    }
}
