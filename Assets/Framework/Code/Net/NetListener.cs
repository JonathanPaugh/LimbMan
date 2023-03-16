using System;
using System.Collections.Generic;

namespace JapeNet
{
	public class NetListener
    {
        private List<Action<object>> listeners = new();

        public int Open(Action<object> action)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i] == null)
                {
                    listeners[i] = action;
                    return i;
                }
            }

            listeners.Add(action);
            return listeners.Count - 1;
        }

        public void Close(int index)
        {
            listeners[index] = null;
        }

        public Action<object> Receive(int index)
        {
            Action<object> action = listeners[index];
            return action;
        }
    }
}