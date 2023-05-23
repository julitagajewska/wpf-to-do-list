using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.MVVM.Model
{
    public static class Messenger
    {
        private static Dictionary<string, Action<object>> _subscribers = new Dictionary<string, Action<object>>();

        public static void Subscribe(string message, Action<object> action)
        {
            if (!_subscribers.ContainsKey(message))
                _subscribers[message] = action;
            else
                _subscribers[message] += action;
        }

        public static void Unsubscribe(string message, Action<object> action)
        {
            if (_subscribers.ContainsKey(message))
                _subscribers[message] -= action;
        }

        public static void Publish(string message, object parameter = null)
        {
            if (_subscribers.ContainsKey(message))
                _subscribers[message]?.Invoke(parameter);
        }
    }
}
