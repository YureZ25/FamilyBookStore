using System.Diagnostics.CodeAnalysis;

namespace Data.Entities.Contracts
{
    public interface ICommonEntity : IEntity
    {
        int Id { get; set; }
    }

    public class CommonEntityEqualityComparer : IEqualityComparer<ICommonEntity>
    {
        public bool Equals(ICommonEntity x, ICommonEntity y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] ICommonEntity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
