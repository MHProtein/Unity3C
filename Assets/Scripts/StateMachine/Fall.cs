using System;
using UnityEngine;

namespace Unity3C.StateMachine
{
    [CreateAssetMenu(fileName = "Fall", menuName = "State/Fall")]
    public class Fall : State
    {
        private void Awake()
        {
            stateName = "Fall";
        }
    }
}