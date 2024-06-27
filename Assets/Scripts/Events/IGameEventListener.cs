

namespace Unity3C.Events
{
    public interface IGameEventListener<T>
    {
        public void OnEventRaised(T item);
    }
}