﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Extensions.Tests
{
    public partial class UnityExtensionsTests
    {
        #region Resolve

        [TestMethod]
        public void Resolve_Generic()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve<IUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve<IUnityContainer>());
        }

        [TestMethod]
        public void Resolve_GenericWithName()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve<IUnityContainer>(Name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(Name, container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve<IUnityContainer>(Name));
        }

        [TestMethod]
        public void Resolve()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve(typeof(IUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);   // TODO: should be null
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve(typeof(IUnityContainer)));
        }

        #endregion


        #region Resolve All

        [TestMethod]
        public void Resolve_AllGeneric()
        {
            // Arrange
            container.Add(typeof(object[]), new object[] { new object() } as object);

            // Act
            container.ResolveAll<object>();

            // Validate
            Assert.AreEqual(typeof(object[]), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll<IUnityContainer>());
        }

        [TestMethod]
        public void Resolve_All()
        {
            // Arrange
            container.Add(typeof(bool[]), new bool[] { true, true, false } as object);

            // Act
            container.ResolveAll(typeof(bool));

            // Validate
            Assert.AreEqual(typeof(bool[]), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll(typeof(IUnityContainer)));
            Assert.ThrowsException<ArgumentNullException>(() => container.ResolveAll(null));
        }

        [TestMethod]
        public void Resolve_AllEnumerable()
        {
            // Arrange
            container.Add(typeof(int[]), new int[] { 0, 1 } as object);

            // Act
            container.ResolveAll(typeof(int));

            // Validate
            Assert.AreEqual(typeof(int[]), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll(typeof(int)));
            Assert.ThrowsException<ArgumentNullException>(() => container.ResolveAll(null));
        }

        #endregion


        #region BuildUp

        [TestMethod]
        public void BuildUp_Generic()
        {
            // Act
            container.BuildUp<IUnityContainer>(container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp<IUnityContainer>(container));
            Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp<IUnityContainer>(null));
        }

        [TestMethod]
        public void BuildUp_WithNameGeneric()
        {
            // Act
            container.BuildUp<IUnityContainer>(container, Name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(Name, container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp<IUnityContainer>(container, Name));
            Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp<IUnityContainer>(null, Name));
        }

        [TestMethod]
        public void BuildUp()
        {
            // Act
            container.BuildUp(typeof(IUnityContainer), container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp(typeof(IUnityContainer), container));
            // TODO:  Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp(typeof(IUnityContainer), (object)null));
        }

        #endregion
    }
}