using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LambdaExpressionAccelerator
{
    public class ModificatorLambdaExpression : ExpressionVisitor
    {
        private MethodInfo _funcF;
        private Dictionary<string, Info> _container;


        public LambdaExpression Optimization<T>(Expression<T> expression, MethodInfo funcF, out IEnumerable<LambdaExpression> preLaunch)
        {
            //Инициализация private properties
            _funcF = funcF;
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
            if (node.Method.Equals(_funcF))
            {
                return ModifyExpressionToParameter(node);
            }
            return node;
        }

        private ParameterExpression ModifyExpressionToParameter(MethodCallExpression node)
        {
            var key = node.Arguments.FirstOrDefault().ToString();
            if (!_container.ContainsKey(key))
            {
                var param = Expression.Parameter(_funcF.ReturnType, "_" + Guid.NewGuid().ToString());
                _container.Add(key, new Info(param, node));
            }
            return _container[key].parameter;
        }
    }
}