using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace SpeedTest
{
    [TestClass]
    public class UnitTest1
    {
        protected int count = 0;
        //Два разных лямбда выражения, для демонстрации, что функция F может быть произвольной 
        private Expression<Func<int,int,int,int>> lambda1;
        private Expression<Func<string, int, bool>> lambda2;
        private int result1;

        public int FuncF1(params object[] parameters)
        {
            this.count++;//Счетчик количества вызовов
            Thread.Sleep(1000);//Задержка для наглядности в скорости расчета
            return parameters.Length > 0 ? (int)parameters[parameters.Length - 1] : 0;
        }

        public bool FuncF2(string str, int i)
        {
            this.count++;//Счетчик количества вызовов
            Thread.Sleep(2000);//Задержка для наглядности в скорости расчета
            return str == i.ToString();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.lambda1 = (x, y, z) => x + FuncF1(x, y, z) + FuncF1() + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(x) + FuncF1(y) + FuncF1(z) + FuncF1(y * 2) + (FuncF1(x) > FuncF1(y) ? FuncF1(x) : FuncF1(x) < FuncF1(y * 2) ? FuncF1(y * 2) : FuncF1(y));
            this.result1 = 23;//результат выполнения данного лямбда выражения с параметрами 1, 2, 3
            this.lambda2 = (s1, x1) => FuncF2(s1, x1) && FuncF2(s1, x1) && FuncF2(s1, x1) && !FuncF2(s1, x1);//Всегда вернет False
        }

        [TestMethod]
        public void Optimization1()
        {
            var result = Helper.RunOptimally(this.lambda1, FuncF1, 1, 2, 3);
            Assert.AreEqual(this.result1, result);
            Assert.AreEqual(6, this.count);//Функция вызывается 6 раз, время выполнения около 6 секунд
        }

        [TestMethod]
        public void Optimization2()
        {
            var result = Helper.RunOptimally(this.lambda2, FuncF2, "1", 1);
            Assert.IsFalse((bool)result);
            Assert.AreEqual(1, this.count);//Функция вызывается 1 раз, время выполнения около 2 секунд
        }

        [TestMethod]
        public void WithoutOptimization1()
        {
            var result = this.lambda1.Compile()(1, 2, 3);
            Assert.AreEqual(this.result1, result);
            Assert.AreEqual(16, this.count);//Функция вызывается 16 раз, время выполнения около 16 секунд
        }

        [TestMethod]
        public void WithoutOptimization2()
        {
            var result = this.lambda2.Compile()("1", 1);
            Assert.IsFalse(result);
            Assert.AreEqual(4, this.count);//Функция вызывается 4 раза, время выполнения около 8 секунд

        }

    }
}