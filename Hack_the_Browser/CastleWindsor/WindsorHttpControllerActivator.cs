using System;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;
using System.Net.Http;

namespace CastleWindsor
{
    //public class WindsorHttpControllerActivator : IHttpControllerActivator
    //{
    //    private readonly IWindsorContainer _container;

    //    public WindsorHttpControllerActivator(IWindsorContainer container)
    //    {
    //        this._container = container;
    //    }

    //    public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor,
    //        Type controllerType)
    //    {
    //        var controller = (IHttpController)this._container.Resolve(controllerType);
    //        request.RegisterForDispose(new Release(() => this._container.Release(controller)));
    //        return controller;
    //    }

    //    private class Release : IDisposable
    //    {
    //        private readonly Action _release;

    //        public Release(Action release)
    //        {
    //            this._release = release;
    //        }

    //        public void Dispose()
    //        {
    //            this._release();
    //        }
    //    }
    //}
}
