using System;
using System.Collections.Generic;
using ServiceLocator.Interfaces;

namespace ServiceLocator.Library
{
    public class ServiceProvider : ServiceLocator.Interfaces.IServiceProvider
    {
        private Dictionary<Type, Func<object>> services;
        private Dictionary<Type, object> scopeservices;

        public ServiceProvider(Dictionary<Type, Func<object>> services, Dictionary<Type, object> scopeservices)
        {
            this.services = services;
            this.scopeservices = scopeservices;
        }

        public object GetService(Type serviceType)
        {
            if (!this.scopeservices.ContainsKey(serviceType))
            {
                this.scopeservices[serviceType] = this.services[serviceType]();
            }
            return this.scopeservices[serviceType];
        }
    }

    public class ServiceScope : IServiceScope
    {
        public ServiceLocator.Interfaces.IServiceProvider ServiceProvider { get; private set; }
        private Dictionary<Type, object> scopeservices = new Dictionary<Type, object>();

        public ServiceScope(Dictionary<Type, Func<object>> services)
        {
            this.ServiceProvider = new ServiceProvider(services, this.scopeservices);
        }

        public void Dispose()
        {
            foreach (var service in scopeservices.Values)
            {
                (service as IDisposable)?.Dispose();
            }
            scopeservices.Clear();
        }
    }

    public class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Dictionary<Type, Func<object>> services;
        public ServiceScopeFactory(Dictionary<Type, Func<object>> services) => this.services = services;
        public IServiceScope CreateScope() => new ServiceScope(this.services);
    }
}