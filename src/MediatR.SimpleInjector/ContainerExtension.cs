namespace MediatR.SimpleInjector
{
    using System.Collections.Generic;
    using System.Reflection;
    using global::SimpleInjector;

    public static class ContainerExtension
    {
        public static Container BuildMediator(this Container container, params Assembly[] assemblies)
        {
            return BuildMediator(container, (IEnumerable<Assembly>)assemblies);
        }

        public static Container BuildMediator(this Container container, IEnumerable<Assembly> assemblies)
        {
            var allAssemblies = new List<Assembly> { typeof(IMediator).GetTypeInfo().Assembly };
            allAssemblies.AddRange(assemblies);

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), allAssemblies);

            container.Collection.Register(typeof(INotificationHandler<>), allAssemblies);

            container.RegisterInstance(new ServiceFactory(container.GetInstance));

            return container;
        }
    }
}
