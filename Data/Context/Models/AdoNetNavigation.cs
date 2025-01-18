using System.Reflection;

namespace Data.Context.Models
{
    internal class AdoNetNavigation
    {
        public PropertyInfo ForeignKey { get; set; }
        public PropertyInfo Navigation { get; set; }
    }
}
