using System;
using System.Collections.Generic;

namespace Events
{
    // SOLID 패턴용 빈 인터페이스
    public interface IGameEvent { }
    
    public static class GameEvents
    {
        private static readonly Dictionary<Type, Delegate> _listeners = new();

        public static void AddListener<T>(Action<T> listener) where T : IGameEvent
        {
            Type type = typeof(T);
            if (!_listeners.ContainsKey(type)) _listeners[type] = listener;
            else _listeners[type] = Delegate.Combine(_listeners[type], listener);
        }

        public static void RemoveListener<T>(Action<T> listener) where T : IGameEvent
        {
            Type type = typeof(T);
            if (_listeners.ContainsKey(type))
            {
                _listeners[type] = Delegate.Remove(_listeners[type], listener);
                if (_listeners[type] == null) _listeners.Remove(type);
            }
        }

        public static void SendEvent<T>(T eventData) where T : IGameEvent
        {
            Type type = typeof(T);
            if (_listeners.TryGetValue(type, out var del))
            {
                (del as Action<T>)?.Invoke(eventData);
            }
        }
        
        public static void MoveSceneRemoveAllListner()
        {
            _listeners.Clear();
        }
    }
}