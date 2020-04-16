﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DNVGL.Common.Core.Patterns
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(taskFactory)
        {
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
}
