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

        public async Task<IEnumerable<Genre>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT 
                    Genres.Id, 
                    Genres.Name 
                FROM Genres
                """)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<Genre> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT 
                    Genres.Id, 
                    Genres.Name 
                FROM Genres
                WHERE Genres.Id = @id
                """)
                .WithParameter(e => e.Id, id)
                .Build();

            return await cmd.FirstOrDefault(cancellationToken);
        }

        public void Insert(Genre genre)
        {
            _dbContext.CreateCommand(genre)
                .WithText("""
                INSERT INTO Genres (Name) 
                VALUES (@name); 
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.Name)
                .Build();
        }

        public void Update(Genre genre)
        {
            _dbContext.CreateCommand(genre)
                .WithText("""
                UPDATE Genres 
                SET Name = @name 
                WHERE Id = @id
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Name)
                .Build();
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand<Genre>(null)
                .WithText("DELETE Genres WHERE Id = @id")
                .WithParameter(e => e.Id, id)
                .Build();
        }

        private Genre Map(DbDataReader reader)
        {
            return new Genre
            {
                Id = reader.Map<int>(nameof(Genre.Id)),
                Name = reader.Map<string>(nameof(Genre.Name)),
            };
        }
    }
}
