namespace Sender.Services.IoC
{
    internal class SimpleContainer
    {
        private readonly Dictionary<Type, List<Func<object>>> _factories = new();

        public void Register<T>(Func<T> factory)
        {
            Type type = typeof(T);

            if (!_factories.ContainsKey(type))
            {
                _factories[type] = new List<Func<object>>();
            }

            _factories[type].Add(() => factory()!);
        }

        public void RegisterSingleton<T>(T instance)
        {
            Register(() => instance);
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);

            if (!_factories.ContainsKey(type) || _factories[type].Count == 0)
            {
                throw new InvalidOperationException($"Тип {type.Name} не зарегистрирован в контейнере");
            }

            return (T)_factories[type][^1]();
        }

        public IReadOnlyList<T> ResolveAll<T>()
        {
            Type type = typeof(T);

            if (!_factories.ContainsKey(type))
            {
                return Array.Empty<T>();
            }

            return _factories[type].Select(factory => (T)factory()).ToList();
        }
    }
}
