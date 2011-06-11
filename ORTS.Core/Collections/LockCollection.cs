﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ORTS.Core.Collections
{
    public class LockCollection<T>
    {
        private readonly HashSet<T> items = new HashSet<T>();
        private readonly object padlock = new object();

        public bool Contains(T item)
        {
            lock (padlock)
            {
                return items.Contains(item);
            }
        }

        public bool Add(T item)
        {
            lock (padlock)
            {
                return items.Add(item);
            }
        }

        public void WaitForNonExistence(T item)
        {
            lock (padlock)
            {
                while (items.Contains(item))
                {
                    Monitor.Wait(padlock);
                }
            }
        }

        public void WaitForAndAdd(T item)
        {
            lock (padlock)
            {
                WaitForNonExistence(item);
                items.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (padlock)
            {
                if (items.Remove(item))
                {
                    Monitor.PulseAll(padlock);
                }
            }
        }
    }
}
