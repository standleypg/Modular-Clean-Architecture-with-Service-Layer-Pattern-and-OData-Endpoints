using System.Linq.Expressions;

namespace RetailPortal.Service.Extensions;

public static class ODataExpressionExtensions
{
    extension(Expression expression)
    {
        public Expression RemoveODataSkipTop()
        {
            return expression switch
            {
                MethodCallExpression { Method.Name: "Skip" or "Take" } methodCall
                    => methodCall.Arguments[0].RemoveODataSkipTop(),

                MethodCallExpression methodCall
                    => methodCall.Update(
                        methodCall.Object?.RemoveODataSkipTop(),
                        methodCall.Arguments.Select(arg => arg.RemoveODataSkipTop())
                    ),

                _ => expression
            };
        }
    }
}