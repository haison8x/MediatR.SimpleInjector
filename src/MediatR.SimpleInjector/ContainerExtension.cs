using SimpleInjector;
using System.Collections.Generic;
using System.Reflection;

namespace MediatR.SimpleInjector
{
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
            container.Register(typeof(IAsyncRequestHandler<,>), allAssemblies);
            container.Register(typeof(IRequestHandler<>), allAssemblies);
            container.Register(typeof(IAsyncRequestHandler<>), allAssemblies);
            container.Register(typeof(ICancellableAsyncRequestHandler<>), allAssemblies);
            container.RegisterCollection(typeof(INotificationHandler<>), allAssemblies);
            container.RegisterCollection(typeof(IAsyncNotificationHandler<>), allAssemblies);
            container.RegisterCollection(typeof(ICancellableAsyncNotificationHandler<>), allAssemblies);
            container.RegisterCollection(typeof(IPipelineBehavior<,>), allAssemblies);

            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));

            return container;
        }
    }
}
