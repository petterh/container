﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Benchmarks
{
    [TestClass]
    public class ResolutionBenchmarksTests
    {
        #region Scaffolding

        protected IUnityContainer Container;

        [TestInitialize]
        public void GlobalSetup()
        {
            Container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .RegisterType(typeof(List<object>))
                .CreateChildContainer()
                .CreateChildContainer();
        }

        #endregion


        #region Interfaces

        [TestMethod]
        public void Resolve_IUnityContainer()
        {
            Assert.IsNotNull(Container.Resolve(typeof(IUnityContainer), null));
        }

        [TestMethod]
        public void Resolve_IServiceProvider()
        {
            Assert.IsNotNull(Container.Resolve(typeof(IServiceProvider), null));
        }

        [TestMethod]
        public void Resolve_IUnityContainerAsync()
        {
            Assert.IsNotNull(((IUnityContainerAsync)Container)
                .ResolveAsync(typeof(IUnityContainerAsync), (string)null)
                .GetAwaiter()
                .GetResult());
        }

        #endregion

        [Ignore]
        [TestMethod]
        public void Resolve_Registered()
        {
            Assert.IsNotNull(Container.Resolve(typeof(List<object>), null));
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Object()
        {
            Assert.IsNotNull(Container.Resolve(typeof(object), null));
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Object_Twice()
        {
            var instance1 = Container.Resolve(typeof(object), null);
            var instance2 = Container.Resolve(typeof(object), null);

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Generic()
        {
            Assert.IsNotNull(Container.Resolve(typeof(List<int>), null));
        }
    }
}
