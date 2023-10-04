using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Data.Repos
{
    internal class AuthorRepo : IAuthorRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public AuthorRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Authors");

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var authors = new List<Author>();
            while (await reader.ReadAsync(cancellationToken))
            {
                authors.Add(Map(reader));
            }
            return authors;
        }

        public async Task<Author> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Authors WHERE Id = @id")
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void Insert(Author author)
        {
            _dbContext.CreateCommand()
                .WithText(@"INSERT INTO Authors (FirstName, LastName) 
                    VALUES (@firstName, @lastName); 
                    SET @id = SCOPE_IDENTITY();")
                .WithParameter("id", author.Id, ParameterDirection.Output)
                .WithParameter("firstName", author.FirstName)
                .WithParameter("lastName", author.LastName);
        }

        public void Update(Author author)
        {
            _dbContext.CreateCommand()
                .WithText(@"UPDATE Authors 
                    SET FirstName = @firstName, LastName = @lastName 
                    WHERE Id = @id")
                .WithParameter("id", author.Id)
                .WithParameter("firstName", author.FirstName)
                .WithParameter("lastName", author.LastName);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Authors WHERE Id = @id")
                .WithParameter("id", id);
        }

        private static Author Map(DbDataReader reader)
        {
            return new Author
            {
                Id = reader.GetInt32(nameof(Author.Id)),
                FirstName = reader.GetString(nameof(Author.FirstName)),
                LastName = reader.GetString(nameof(Author.LastName)),
            };
        }
    }
}
