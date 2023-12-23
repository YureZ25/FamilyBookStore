using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Services.ModelBinders
{
    public class IsbnModelBinder : IModelBinder
    {
        private readonly IsbnTypeConverter _typeConverter = new();

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == ValueProviderResult.None) return Task.CompletedTask;
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            try
            {
                string text = value.FirstValue;

                object model = !string.IsNullOrWhiteSpace(text) ? _typeConverter.ConvertFrom(null, value.Culture, text) : null;

                if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(value.ToString()));
                    return Task.CompletedTask;
                }

                bindingContext.Result = ModelBindingResult.Success(model);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
