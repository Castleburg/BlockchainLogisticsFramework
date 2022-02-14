using System;
using System.Collections.Generic;
using System.Text;

namespace ManualTests
{
    class Class1
    {

        public static void Main(string[] args)
        {
            List<DateTime> TestList = new List<DateTime>
            {
                new DateTime(5020102131),
                new DateTime(2020102131),
                new DateTime(3020102131)
            };

            TestList.Sort((x, y) => y.CompareTo(x));
            Console.WriteLine("test");
        }
    }
}
