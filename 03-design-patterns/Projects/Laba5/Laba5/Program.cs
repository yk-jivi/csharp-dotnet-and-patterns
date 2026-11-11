using IRC.dll;
using RC.dll;

namespace Laba5
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var generator = new RCGenerator();
            var handler = new RCHandler();
            var rch = new RChain(generator, handler);

            rch.Use((context, next) =>
            {
                context.sum();
                Console.WriteLine($"-> Прямой ход: sum. Z = {context.Z}");
                bool rc = next(context);
                return rc;
            })
            .Use((context, next) =>
            {
                context.sub();
                Console.WriteLine($"-> Прямой ход: sub. Z = {context.Z}");
                bool rc = next(context);
                return rc;
            })
            .Use((context, next) =>
            {
                context.div(); // прямой
                Console.WriteLine($"-> Прямой ход нового элемента: div. Z = {context.Z}");

                bool rc = next(context);

                context.div(); // обратный
                Console.WriteLine($"<- Обратный ход нового элемента: div. Z = {context.Z}");
                return rc;
            })
            .Use((context, next) =>
            {
                bool rc = next(context);
                context.mul();
                Console.WriteLine($"<- Обратный ход: mul. Z = {context.Z}");
                return rc;
            });

            generator.Start(new RCGenerator.Context(3, 5));
            Console.WriteLine("-------------------------------------");
            var c = new RCGenerator.Context(10, 5);
            generator.Start(c);
            generator.Start(new RCGenerator.Context(4, 6, c.Z));
            generator.Start(new RCGenerator.Context());
        }
    }
}