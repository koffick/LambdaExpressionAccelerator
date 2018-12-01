using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LambdaExpressionAccelerator
{
    public class UniversalModificatorLambdaExpression : ExpressionVisitor
    {
        private Dictionary<string, Info> _container;


        public LambdaExpression Optimization<T>(Expression<T> expression,out IEnumerable<LambdaExpression> preLaunch)
        {
            //Инициализация private properties
            _container = new Dictionary<string, Info>();

            //Модификация body исходного выражения
            var exp = (Expression<T>)Visit(expression);

            //Создание списка уникальных вызовов функции F с различными комбинациями параметров
            preLaunch = MakePreLaunch(exp.Parameters.ToList());

            //Переопределение параметров оптимизированного выражения
            var param = MakeParams(exp.Parameters.ToList());

            return Expression.Lambda(exp.Body, param);
        }

        private IEnumerable<ParameterExpression> MakeParams(List<ParameterExpression> parameters)
        {
            foreach (var item in _container)
            {
                parameters.Add(item.Value.parameter);
            }
            return parameters;
        }

        private IEnumerable<LambdaExpression> MakePreLaunch(List<ParameterExpression> parameters)
        {
            var preLaunch = new List<LambdaExpression>();
            foreach (var item in _container)
            {
                var body = item.Value.method;
                var newExpression = Expression.Lambda(body, parameters);
                preLaunch.Add(newExpression);
            }
            return preLaunch;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var key = MakeKey(node);

            if (!_container.ContainsKey(key))
            {
                var param = Expression.Parameter(node.Method.ReturnType, "_" + Guid.NewGuid().ToString());
                _container.Add(key, new Info(param, node));
            }
            return _container[key].parameter;
        }

        private static string MakeKey(MethodCallExpression node)
        {
            var key = "";
            foreach (var item in node.Arguments)
            {
                key = key + item.ToString();
            }
            return key + node.Method.Name;
        }
    }
}