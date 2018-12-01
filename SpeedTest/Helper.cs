using LambdaExpressionAccelerator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SpeedTest
{
    public static partial class Helper
    {
        public static object RunOptimally<T>(Expression<T> expression, MethodInfo funcF, params object[] numbers)
        {
            IEnumerable<LambdaExpression> preLaunch;
            var modifiedExpression = new ModificatorLambdaExpression().Optimization(expression, funcF, out preLaunch);
            var addParams = new List<object>();
            foreach (var item in preLaunch)
            {
                addParams.Add(item.Compile().DynamicInvoke(numbers));
            }
            var newParams = numbers.Concat(addParams.ToArray());
            return modifiedExpression.Compile().DynamicInvoke(newParams);
        }

        public static object RunOptimally<T>(Expression<T> expression, params object[] numbers)
        {
            IEnumerable<LambdaExpression> preLaunch;
            var modifiedExpression = new UniversalModificatorLambdaExpression().Optimization(expression, out preLaunch);
            var addParams = new List<object>();
            foreach (var item in preLaunch)
            {
                addParams.Add(item.Compile().DynamicInvoke(numbers));
            }
            var newParams = numbers.Concat(addParams.ToArray());
            return modifiedExpression.Compile().DynamicInvoke(newParams);
        }

        private static object[] Concat(this object[] left, object[] right)
        {
            var oldLength = left.Length;
            Array.Resize<object>(ref left, oldLength + right.Length);
            Array.Copy(right, 0, left, oldLength, right.Length);
            return left;
        }
    }
}
