using System.Linq.Expressions;

namespace SpeedTest
{
    public delegate int ParamsFunc1(params object[] parameters);
    public static partial class Helper
    {
        public static object RunOptimally<T>(Expression<T> expression, ParamsFunc1 funcF, params object[] numbers)
        {
            return RunOptimally(expression, funcF.Method, numbers);
        }
    }

    public delegate bool ParamsFunc2(string str, int i);
    public static partial class Helper
    {
        public static object RunOptimally<T>(Expression<T> expression, ParamsFunc2 funcF, params object[] numbers)
        {
            return RunOptimally(expression, funcF.Method, numbers);
        }
    }

}
