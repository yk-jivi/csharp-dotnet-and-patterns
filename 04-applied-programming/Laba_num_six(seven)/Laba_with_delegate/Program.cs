using Lab6Lib;

namespace Laba
{
    public class Program
    {
        static void Main(string[] args)
        {
            PublisherWithDelegate event1 = new PublisherWithDelegate("EVENT-1");
            PublisherWithDelegate event2 = new PublisherWithDelegate("EVENT-2");
            PublisherWithDelegate event3 = new PublisherWithDelegate("EVENT-3");

            SubscriberA subscriberA = new SubscriberA();
            SubscriberB subscriberB = new SubscriberB();
            SubscriberC subscriberC = new SubscriberC();

            Console.WriteLine("--------------------Тест 1 ------------------");
            event1.subscribe(subscriberA.update);
            event2.subscribe(subscriberA.update);
            event3.subscribe(subscriberA.update);
            event1.nonify(); event2.nonify(); event3.nonify();

            Console.WriteLine("--------------------Тест 2 ------------------");
            event1.subscribe(subscriberB.update);
            event2.unsubscribe(subscriberA.update);
            event1.nonify(); event2.nonify(); event3.nonify();

            Console.WriteLine("--------------------Тест 3 ------------------");
            event2.subscribe(subscriberB.update);
            event3.subscribe(subscriberC.update);
            event1.nonify(); event2.nonify(); event3.nonify();

            Console.WriteLine("--------------------Тест 4 ------------------");
            event2.subscribe(subscriberB.update);
            event3.subscribe(subscriberC.update);
            event1.nonify(); event2.nonify(); event3.nonify();

            Console.WriteLine("--------------------Тест 5 ------------------");
            event1.unsubscribe(subscriberA.update);
            event1.unsubscribe(subscriberB.update);
            event2.unsubscribe(subscriberB.update);
            event3.unsubscribe(subscriberA.update);
            event3.unsubscribe(subscriberC.update);
            event1.nonify(); event2.nonify(); event3.nonify();
        }
    }
    public class SubscriberA
    {
        public void update(string eventname)
        {
            Console.WriteLine($"Subscriber:A, Event:{eventname}");
        }
    }
    public class SubscriberB
    {
        public void update(string eventname)
        {
            Console.WriteLine($"Subscriber:B, Event:{eventname}");
        }
    }
    public class SubscriberC
    {
        public void update(string eventname)
        {
            Console.WriteLine($"Subscriber:C, Event:{eventname}");
        }
    }
}
