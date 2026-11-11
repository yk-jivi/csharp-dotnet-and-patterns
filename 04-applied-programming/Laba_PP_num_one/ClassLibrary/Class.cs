using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
      
        internal class Iamhere
        {
            public int Composition(int a, int b)
            {
                return a * b;
            }
        }
        public class Hello
        {
            public void Sum(int a, int b)
            {
                Console.Write("Cумма значений: ");
                Console.WriteLine(a + b);
            }
            public int CompositionFromInternalClass(int a, int b)
            {
                Iamhere helper = new Iamhere();
                return helper.Composition(a, b);
            }
        }
}
