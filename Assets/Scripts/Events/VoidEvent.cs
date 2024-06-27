using UnityEngine;

namespace Unity3C.Events
{
    [CreateAssetMenu(fileName = "New Void Event", menuName = "GameEvents/VoidEvent")]
    public class VoidEvent : BaseGameEvent<Void>
    {
        public void Raise()
        {
            Raise(new Void());
        }
    }
}