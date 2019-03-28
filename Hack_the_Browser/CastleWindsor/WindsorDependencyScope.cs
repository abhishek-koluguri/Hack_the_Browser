using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;

namespace CastleWindsor
{
    /// <summary>
    /// 
    /// </summary>
    public class WindsorDependencyScope : IDependencyScope
    {
        private readonly IKernel _container;

        private readonly IDisposable _scope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public WindsorDependencyScope(IKernel container)
        {
            _container = container;
            _scope = container.BeginScope();
        }

        public object GetService(Type serviceType)
        {
            return _container.HasComponent(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
