using System;

namespace ServiceLocator.Interfaces
{
    public enum ServiceLifetime { Transient, Singleton, Scoped }

    public interface IServiceProvider
    {
        object GetService(Type serviceType);
    }

    public interface IServiceScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }

    public interface IServiceScopeFactory
    {
        IServiceScope CreateScope();
    }
}