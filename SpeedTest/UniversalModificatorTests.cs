using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Threading;

namespace SpeedTest
{
    //Тестирование универсального модификатора лямбда. Это уже немного за рамками поставленной задачи.
    //Данный модификатор позволяет не использовать делегаты, а также позволяет оптимизировать лямбду, содержащую несколько вызовов РАЗНЫХ функций, а не одной и той же F, как было в условии.
    [TestClass]
    public class UniversalModificatorTests
    {
        protected int count1 = 0;
        protected int count2 = 0;
        private Expression<Func<int, int, int, int>> lambda;
        private int result;

        public int FuncF1(params object[] parameters)
        {
            this.count1++;//Счетчик количества вызовов
            Thread.Sleep(1000);//Задержка для наглядности в скорости расчета
            return parameters.Length > 0 ? (int)parameters[parameters.Length - 1] : 0;
        }

        public int FuncF2(int i, int j)
        {
            this.count2++;//Счетчик количества вызовов
            Thread.Sleep(2000);//Задержка для наглядности в скорости расчета
            return i * j;
        }

        [TestInitialize]
        public void SetUp()
        {
            this.lambda = (x, y, z) => x + FuncF1(x, y, z) + FuncF2(x, y) + FuncF2(x, y) + FuncF2(x, z) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(y) + FuncF1(z) + FuncF1(y * 2) + (FuncF1(x) > FuncF1(y) ? FuncF1(x) : FuncF1(x) < FuncF1(y * 2) ? FuncF1(y * 2) : FuncF1(y));
            this.result = 29;//результат выполнения данного лямбда выражения с параметрами 1, 2, 3
        }

        [TestMethod]
        public void Optimization()
        {
            var result = Helper.RunOptimally(this.lambda, 1, 2, 3);
            Assert.AreEqual(this.result, result);
            Assert.AreEqual(5, this.count1);//Функция1 вызывается 5 раз
            Assert.AreEqual(2, this.count2);//Функция2 вызывается 2 раза
            // время выполнения около 9 секунд
        }

        [TestMethod]
        public void WithoutOptimization()
        {
            var result = this.lambda.Compile()(1, 2, 3);
            Assert.AreEqual(this.result, result);
            Assert.AreEqual(14, this.count1);//Функция1 вызывается 14 раз
            Assert.AreEqual(3, this.count2);//Функция2 вызывается 3 раза
            // время выполнения около 20 секунд
        }
    }
}
