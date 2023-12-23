using Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Services.ModelBinders
{
    public class IsbnModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(ISBN) || context.Metadata.ModelType == typeof(ISBN?))
            {
                return new IsbnModelBinder();
            }

            return null;
        }
    }
}
