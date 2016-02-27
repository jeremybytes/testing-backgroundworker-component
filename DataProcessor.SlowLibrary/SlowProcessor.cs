using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SlowLibrary
{
    /// <summary>
    /// The ProcessModel class implements the IEnumerable interface
    /// which allows us to use the object in a foreach loop.
    /// To simulate a long-running process, the enumerator contains
    /// a call to Sleep() in the MoveNext() method.
    /// </summary>
    /// <example>
    ///   var model = new ProcessModel(100);
    ///   foreach (var item  in model)
    ///   {
    ///     Console.WriteLine(item.ToString());
    ///   }
    /// </example>

    public class SlowProcessor : IEnumerable<int>
    {
        private int currentPosition;
        private int totalIterations;

        public SlowProcessor(int iterations)
        {
            totalIterations = iterations;
        }

        public IEnumerator<int> GetEnumerator()
        {
            currentPosition = 0;
            while (currentPosition < totalIterations)
            {
                Thread.Sleep(100);
                currentPosition++;
                yield return currentPosition;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
