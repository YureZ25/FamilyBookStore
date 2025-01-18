using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class AuthorRepo : IAuthorRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public AuthorRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Author>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT
                    Authors.Id,
                    Authors.FirstName,
                    Authors.LastName
                FROM Authors
                """)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<Author> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT
                    Authors.Id,
                    Authors.FirstName,
                    Authors.LastName
                FROM Authors
                WHERE Authors.Id = @id
                """)
                .WithParameter(e => e.Id, id)
                .Build();

            return await cmd.FirstOrDefault(cancellationToken);
        }

        public void Insert(Author author)
        {
            _dbContext.CreateCommand(author)
                .WithText("""
                INSERT INTO Authors (FirstName, LastName) 
                VALUES (@firstName, @lastName); 
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.FirstName)
                .WithParameter(e => e.LastName)
                .Build();
        }

        public void Update(Author author)
        {
            _dbContext.CreateCommand(author)
                .WithText("""
                UPDATE Authors 
                SET 
                    FirstName = @firstName, 
                    LastName = @lastName 
                WHERE Id = @id;
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.FirstName)
                .WithParameter(e => e.LastName)
                .Build();
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand<Author>(null)
                .WithText("DELETE Authors WHERE Id = @id")
                .WithParameter(e => e.Id, id);
        }

        private static Author Map(DbDataReader reader)
        {
            return new Author
            {
                Id = reader.Map<int>(nameof(Author.Id)),
                FirstName = reader.Map<string>(nameof(Author.FirstName)),
                LastName = reader.Map<string>(nameof(Author.LastName)),
            };
        }
    }
}
