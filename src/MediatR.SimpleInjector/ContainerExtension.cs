using System.IO;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace MediatR.SimpleInjector {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System;
    using global::SimpleInjector;

    public static class ContainerExtension {
        public static Container BuildMediator (this Container container, params Assembly[] assemblies) {
            return BuildMediator (container, (IEnumerable<Assembly>) assemblies);
        }

        public static Container BuildMediator (this Container container, IEnumerable<Assembly> assemblies) {
            var allAssemblies = GetAssemblies(assemblies);

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            RegisterHandlers(container, typeof(INotificationHandler<>), allAssemblies);
            RegisterHandlers(container, typeof(IRequestExceptionAction<,>), allAssemblies);
            RegisterHandlers(container, typeof(IRequestExceptionHandler<,,>), allAssemblies);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new []
            {
                typeof(RequestExceptionProcessorBehavior<,>),
                typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>)
            });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);

            return container;
        }

        private static void RegisterHandlers(Container container, Type collectionType, Assembly[] assemblies)
        {
            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            container.Collection.Register(collectionType, handlerTypes);
        }

        private static Assembly[] GetAssemblies(IEnumerable<Assembly> assemblies)
        {
            var allAssemblies = new List<Assembly> { typeof (IMediator).GetTypeInfo ().Assembly };
            allAssemblies.AddRange (assemblies);
            
            return allAssemblies.ToArray();            
        }

    }
}