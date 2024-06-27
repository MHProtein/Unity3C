﻿

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3C.EventCenter
{
    public class EventCenter<InstanceType, KeyType, DelegateType> 
        where DelegateType : Delegate where InstanceType : EventCenter<InstanceType, KeyType, DelegateType>
    {
        private static InstanceType _instance;

        public static InstanceType Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<InstanceType>();
                    _instance.Init();
                }
                return _instance;
            }
        }
        
        private Dictionary<KeyType, DelegateType> m_events;
        public Dictionary<KeyType, DelegateType> Events { get => m_events; }
        public virtual void Init()
        {
            m_events = new Dictionary<KeyType, DelegateType>();
        }
        
        public virtual bool Raise(KeyType key, params object[] args)
        {
            if (m_events.TryGetValue(key, out DelegateType _delegate))
            {
                _delegate.DynamicInvoke(args);
                return true;
            }

            return false;
        }

        public virtual void Register(KeyType key, DelegateType @delegate)
        {
            if (m_events.TryGetValue(key, out DelegateType outDelegate))
            {
                m_events[key] = (DelegateType)Delegate.Combine(outDelegate, @delegate);
            }
            else
            {
                m_events.Add(key, @delegate);
            }
        }

        public virtual bool Unregister(KeyType key, DelegateType @delegate)
        {
            if (m_events.TryGetValue(key, out DelegateType outDelegate))
            {
                m_events[key] = (DelegateType)Delegate.Remove(outDelegate, @delegate);
                return true;
            }

            return false;
        }

        public virtual bool EventExists(KeyType key, DelegateType @delegate)
        {
            if (m_events.TryGetValue(key, out DelegateType outDelegate))
            {
                Delegate[] delegates = outDelegate.GetInvocationList();
                foreach (var dele in delegates)
                {
                    if (dele.Equals(@delegate))
                        return true;
                }
            }

            return false;
        }
        
    }
}