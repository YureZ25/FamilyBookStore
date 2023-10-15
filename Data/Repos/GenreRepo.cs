using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class GenreRepo : IGenreRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public GenreRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Genres");

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var genres = new List<Genre>();
            while (await reader.ReadAsync(cancellationToken))
            {
                genres.Add(Map(reader));
            }
            return genres;
        }

        public async Task<Genre> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Genres WHERE Id = @id")
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void Insert(Genre genre)
        {
            _dbContext.CreateCommand(genre)
                .WithText(@"INSERT INTO Genres (Name) 
                    VALUES (@name); 
                    SET @id = SCOPE_IDENTITY();")
                .WithParameter("id", genre.Id, ParameterDirection.Output)
                .WithParameter("name", genre.Name);
        }

        public void Update(Genre genre)
        {
            _dbContext.CreateCommand(genre)
                .WithText(@"UPDATE Genres 
                    SET Name = @name 
                    WHERE Id = @id")
                .WithParameter("id", genre.Id)
                .WithParameter("name", genre.Name);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Genres WHERE Id = @id")
                .WithParameter("id", id);
        }

        private Genre Map(DbDataReader reader)
        {
            return new Genre
            {
                Id = reader.GetInt32(nameof(Genre.Id)),
                Name = reader.GetString(nameof(Genre.Name)),
            };
        }
    }
}
