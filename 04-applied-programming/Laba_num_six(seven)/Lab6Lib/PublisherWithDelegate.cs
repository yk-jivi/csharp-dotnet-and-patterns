namespace Lab6Lib
{
    public delegate void NotifyHandler(string eventname);

    public class PublisherWithDelegate
    {
        private string _eventname;
        private NotifyHandler? _subscribers;

        public event NotifyHandler OnNotify
        {
            add
            {
                if (_subscribers == null || !_subscribers.GetInvocationList().Contains(value))
                    _subscribers += value;
            }
            remove
            {
                _subscribers -= value;
            }
        }

        public PublisherWithDelegate(string eventname)
        {
            _eventname = eventname;
        }

        public void subscribe(NotifyHandler subcriber)
        {
            OnNotify += subcriber;
        }

        public bool unsubscribe(NotifyHandler subcriber)
        {
            OnNotify -= subcriber;
            return true;
        }

        public int nonify()
        {
            _subscribers?.Invoke(_eventname);
            return _subscribers?.GetInvocationList().Length ?? 0;
        }
    }
}
