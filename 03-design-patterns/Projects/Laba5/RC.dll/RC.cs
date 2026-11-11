using IRC.dll;

namespace RC.dll
{
    public class RCGenerator : IRCGenerator<RCGenerator.Context>
    {
        public class Context   // полезная нагрузка
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Z { get; private set; }

            public Context(int x = 0, int y = 0, int z = 0) { this.X = x; this.Y = y; this.Z = z; }
            //public Context() : this(0, 0, 0) { }
            public int sum() => this.Z += this.X + this.Y;
            public int sub() => this.Z += this.X - this.Y;
            public int mul() => this.Z += this.X * this.Y;

            // Новый метод XXX (целочисленное деление с защитой от нуля)
            public int div() => this.Y != 0 ? this.Z += this.X / this.Y : this.Z;
        }
        public Func<Context, bool> Next { get; set; } = null!;  // <----- RChain;
        public void Start(Context context)                      // старт цепочки
        {
            Next?.Invoke(context);
            Console.WriteLine("Start: X = {0}, Y = {1}, Z = {2}", context.X, context.Y, context.Z);
        }
    }
    public class RCHandler : IRCHandler<RCGenerator.Context>
    {
        public void LastUse(RCGenerator.Context context)
        {
            Console.WriteLine("LastUse: X = {0}, Y = {1}, Z = {2}", context.X, context.Y, context.Z);
        }
    }
    public class RChain : IResponsibilityChain<RCGenerator.Context>
    {
        private readonly IRCGenerator<RCGenerator.Context> generator;
        private readonly IRCHandler<RCGenerator.Context> handler;
        private readonly List<Func<RCGenerator.Context, Func<RCGenerator.Context, bool>, bool>> pipeline = new();

        public RChain(IRCGenerator<RCGenerator.Context> gen, IRCHandler<RCGenerator.Context> h)
        {
            generator = gen;
            handler = h;
            generator.Next = Execute;
        }

        public IResponsibilityChain<RCGenerator.Context> Use(Func<RCGenerator.Context, Func<RCGenerator.Context, bool>, bool> step)
        {
            pipeline.Add(step);
            return this;
        }

        private bool Execute(RCGenerator.Context ctx)
        {
            int index = 0;
            bool NextStep(RCGenerator.Context c)
            {
                bool rc = false;
                if (index >= pipeline.Count) handler.LastUse(c);
                else rc = pipeline[index++](c, NextStep); // рекурсия
                return rc;
            }
            return NextStep(ctx);
        }
    }
}
