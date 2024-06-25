using System;

namespace Singleton
{
    namespace Singleton
    {
        /// <summary>
        /// A generic singleton class that ensures only one instance of the specified type T exists.
        /// </summary>
        /// <typeparam name="T">The type of the singleton instance. Must be a class with a parameterless constructor.</typeparam>
        public class Singleton<T> where T : class, new()
        {
            protected Singleton() { }

            /// <summary>
            /// A private, static, read-only Lazy instance that holds the singleton instance.
            /// </summary>
            private static readonly Lazy<T> _instance = new(() => new T());

            /// <summary>
            /// Gets the singleton instance of the specified type T.
            /// </summary>
            public static T Instance { get { return _instance.Value; } }
        }
    }
}
