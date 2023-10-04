using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class GenreVM
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }
    }

    internal static class GenreMapper
    {
        public static GenreVM Map(this Genre genre)
        {
            return new GenreVM
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }

        public static Genre Map(this GenreVM genreVM)
        {
            return new Genre
            {
                Id = genreVM.Id,
                Name = genreVM.Name,
            };
        }
    }
}
