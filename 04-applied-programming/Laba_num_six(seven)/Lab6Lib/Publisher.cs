namespace Lab6Lib
{
    public interface ISubscriber
    {
        void update(string eventname);
    }

    public class Publisher
    {

        private string _eventname;
        private List<ISubscriber> _subcribers;

        public Publisher(string eventname)
        {
            _eventname = eventname;
            _subcribers = new List<ISubscriber>();
        }
        public void subscribe(ISubscriber subscriber)
        {
            if (!_subcribers.Contains(subscriber))
            {
                _subcribers.Add(subscriber);
            }
        }
        public bool unsubscribe(ISubscriber subscriber)
        {
            return _subcribers.Remove(subscriber);
        }
        public int nonify()
        {
            foreach(var s in _subcribers) 
            {
                s.update(_eventname);
            }
            return _subcribers.Count;
        }
    }
}
