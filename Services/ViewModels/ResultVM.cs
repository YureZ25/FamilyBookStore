using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Expressions;

namespace Services.ViewModels
{
    public class ResultVM
    {
        public bool Success { get; protected init; }

        public string ErrorKey { get; protected init; }
        public string ErrorMessage { get; protected init; }

        /// <summary>
        /// Success result
        /// </summary>
        public ResultVM()
        {
            Success = true;
            ErrorKey = null;
            ErrorMessage = null;
        }

        /// <summary>
        /// Error result
        /// </summary>
        /// <param name="errorMessage">Error message for the user</param>
        public ResultVM(string errorMessage)
        {
            Success = false;
            ErrorKey = string.Empty;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Error result for specific key
        /// </summary>
        /// <param name="errorKey">Model class + model field error for</param>
        /// <param name="errorMessage">Error message for the user</param>
        public ResultVM(string errorKey, string errorMessage)
        {
            Success = false;
            ErrorKey = errorKey;
            ErrorMessage = errorMessage;
        }
    }

    public class ResultVM<T> : ResultVM
    {
        public T Data { get; protected init; }


        /// <summary>
        /// Success result
        /// </summary>
        /// <param name="data">Data to return</param>
        public ResultVM(T data)
        {
            Success = true;
            Data = data;
            ErrorKey = null;
            ErrorMessage = null;
        }

        /// <summary>
        /// Error result
        /// </summary>
        /// <param name="errorMessage">Error message for the user</param>
        public ResultVM(string errorMessage)
        {
            Success = false;
            Data = default;
            ErrorKey = string.Empty;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Error result for specific key
        /// </summary>
        /// <param name="errorKey">Model class + model field error for</param>
        /// <param name="errorMessage">Error message for the user</param>
        public ResultVM(string errorKey, string errorMessage)
        {
            Success = false;
            ErrorKey = errorKey;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Error result for specific key
        /// </summary>
        /// <param name="errorKeyExpr">Model field selector</param>
        /// <param name="errorMessage">Error message for the user</param>
        public ResultVM(Expression<Func<T, object>> errorKeyExpr, string errorMessage)
        {
            Success = false;
            Data = default;
            ErrorMessage = errorMessage;

            MemberExpression memberExpr = errorKeyExpr.Body as MemberExpression;

            if (memberExpr is null && errorKeyExpr.Body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            {
                memberExpr = unary.Operand as MemberExpression;
            }

            if (memberExpr is null) throw new ArgumentException("Must be member selector", nameof(errorKeyExpr));

            ErrorKey = memberExpr.Member.Name;
            while (memberExpr.Expression is MemberExpression)
            {
                memberExpr = memberExpr.Expression as MemberExpression;
                ErrorKey = memberExpr.Member.Name + "." + ErrorKey;
            }
        }
    }

    public static class ResultVMExtensions
    {
        public static ResultVM ToResultVM(this IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return new();
            }
            else
            {
                return new(string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            }
        }

        public static ResultVM ToResultVM(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid || modelState.ErrorCount == 0) return new();

            var firstErrorField = modelState.First(s => s.Value.Errors.Any());
            var firstError = firstErrorField.Value.Errors.First();

            return new(firstErrorField.Key, firstError.ErrorMessage);
        }
    }
}
