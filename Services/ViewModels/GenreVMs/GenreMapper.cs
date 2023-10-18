using Data.Entities;

namespace Services.ViewModels.GenreVMs
{
    internal static class GenreMapper
    {
        public static GenreGetVM Map(this Genre genre)
        {
            return new GenreGetVM
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }

        public static Genre Map(this GenrePostVM genreVM)
        {
            return new Genre
            {
                Id = genreVM.Id ?? 0,
                Name = genreVM.Name,
            };
        }
    }
}
