﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MxA.Services {
   public class AsyncLazy<T> : Lazy<Task<T>> {
      public AsyncLazy(Func<T> valueFactory) :
          base(() => Task.Factory.StartNew(valueFactory)) { }

      public AsyncLazy(Func<Task<T>> taskFactory) :
          base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

      public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
   }

}
