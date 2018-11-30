using System.Linq.Expressions;

namespace LambdaExpressionAccelerator
{
    public struct Info
    {
        public ParameterExpression parameter;
        public MethodCallExpression method;

        public Info(
            ParameterExpression parameter,
            MethodCallExpression method)
        {
            this.parameter = parameter;
            this.method = method;
        }
    }
}
