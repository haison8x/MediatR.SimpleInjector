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
            var allAssemblies = new List<Assembly> { typeof (IMediator).GetTypeInfo ().Assembly };
            allAssemblies.AddRange (assemblies);

            container.RegisterSingleton<IMediator, Mediator> ();
            container.Register (typeof (IRequestHandler<,>), allAssemblies);

            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var notificationHandlerTypes = container.GetTypesToRegister (typeof (INotificationHandler<>), assemblies, new TypesToRegisterOptions {
                IncludeGenericTypeDefinitions = true,
                    IncludeComposites = false,
            });
            container.Register (typeof (INotificationHandler<>), notificationHandlerTypes);

            container.Collection.Register (typeof (IPipelineBehavior<,>), Enumerable.Empty<Type> ());
            container.Collection.Register (typeof (IRequestPreProcessor<>), Enumerable.Empty<Type> ());
            container.Collection.Register (typeof (IRequestPostProcessor<,>), Enumerable.Empty<Type> ());

            container.Register (() => new ServiceFactory (container.GetInstance), Lifestyle.Singleton);

            return container;
        }
    }
}