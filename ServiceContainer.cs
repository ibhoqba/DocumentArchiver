using System;
using System.Collections.Generic;

namespace DocumentArchiever
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        public void RegisterSingleton<T>(T instance) where T : class
        {
            _singletons[typeof(T)] = instance;
        }

        public void RegisterSingleton<T>(Func<T> factory) where T : class
        {
            _factories[typeof(T)] = () => factory();
        }

        public void RegisterTransient<T>(Func<T> factory) where T : class
        {
            _factories[typeof(T)] = () => factory();
        }

        public T Resolve<T>() where T : class
        {
            if (_singletons.TryGetValue(typeof(T), out var singleton))
                return (T)singleton;

            if (_factories.TryGetValue(typeof(T), out var factory))
                return (T)factory();

            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }
    }
}