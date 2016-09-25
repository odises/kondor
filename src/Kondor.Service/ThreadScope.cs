using System;
using System.Threading;

namespace Kondor.Service
{
    public sealed class ThreadScope : IDisposable
    {
        private readonly bool _skip;
        private readonly string _name;
        private readonly Action _onDisposeCallback;


        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadScope"/> class.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        public ThreadScope(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadScope"/> class.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        /// <param name="onDisposeCallback">The callback which is called when the scope is being disposed.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ThreadScope(string name, Action onDisposeCallback)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }
            
            this._name = name;
            this._skip = IsInScope(name);
            this._onDisposeCallback = onDisposeCallback;
            if (_skip)
                return;
            Thread.SetData(Thread.GetNamedDataSlot(name), true);
        }

        /// <summary>
        /// Determines whether the scope with the specified name has been created and not yet disposed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if the scope with the specified name has been created and not yet disposed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInScope(string name)
        {
            return (bool)(Thread.GetData(Thread.GetNamedDataSlot(name)) ?? false);
        }


        /// <summary>
        /// Removes an indicator from the current thread data.
        /// </summary>
        public void Dispose()
        {
            if (_skip)
                return;

            Thread.SetData(Thread.GetNamedDataSlot(_name), false);
            if (_onDisposeCallback != null)
                _onDisposeCallback();
        }
    }
}
