using System;

namespace DNVGL.Common.Core.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T>
        where T: class
    {
        
        /// <summary>
        /// 
        /// </summary>
        private static class LazyConstructor
        {
            private static T _obj;

            public static Func<T> Factory { get; set; }

            public static T Construct()
            {
                return _obj ?? (_obj = DoConstruction());
            }

            private static T DoConstruction()
            {
                return Factory?.Invoke()
                       ?? (typeof(T).GetConstructor(Type.EmptyTypes) != null
                           ? (T)Activator.CreateInstance(typeof(T))
                           : default);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Func<T> SetFactory(Func<T> factory)
        {
            var oldFactory = LazyConstructor.Factory;

            LazyConstructor.Factory = factory;

            return oldFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public static T Instance => LazyConstructor.Construct();
    }
}
