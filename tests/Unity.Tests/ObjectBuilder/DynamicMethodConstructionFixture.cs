﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.Practices.ObjectBuilder2.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Builder.Selection;
using Unity.Container;
using Unity.Lifetime;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;
using InjectionConstructorAttribute = Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles.InjectionConstructorAttribute;

namespace Unity.Tests.ObjectBuilder
{
    [TestClass]
    public class DynamicMethodConstructionFixture
    {
        [TestMethod]
        public void TheCurrentOperationIsNullAfterSuccessfullyExecutingTheBuildPlan()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(new CurrentOperationSensingResolverPolicy<object>()),
                typeof(ConstructorInjectionTestClass));

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);
            plan.BuildUp(context);

            Assert.IsNull(context.CurrentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileInvokingTheConstructorIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ThrowingConstructorInjectionTestClass));

            IBuildPlanPolicy plan =
                GetPlanCreator(context).CreatePlan(context, context.BuildKey);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(ThrowingConstructorInjectionTestClass.ConstructorException, e);

                var operation = (InvokingConstructorOperation)context.CurrentOperation;
                Assert.IsNotNull(operation);
                Assert.AreSame(typeof(ThrowingConstructorInjectionTestClass), operation.TypeBeingConstructed);
            }
        }

        [TestMethod]
        public void ResolvingAParameterSetsTheCurrentOperation()
        {
            var resolverPolicy = new CurrentOperationSensingResolverPolicy<object>();

            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(resolverPolicy),
                new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass)));

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);
            plan.BuildUp(context);

            Assert.IsNotNull(resolverPolicy.CurrentOperation);
        }

        [TestMethod]
        public void ExceptionThrownWhileResolvingAParameterIsBubbledUpAndTheCurrentOperationIsNotCleared()
        {
            var exception = new ArgumentException();
            var resolverPolicy = new ExceptionThrowingTestResolverPolicy(exception);

            MockBuilderContext context = GetContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(ConstructorInjectionTestClass));

            context.Policies.Set<IConstructorSelectorPolicy>(
                new TestSingleArgumentConstructorSelectorPolicy<ConstructorInjectionTestClass>(resolverPolicy),
                context.BuildKey);

            IBuildPlanPolicy plan = GetPlanCreator(context).CreatePlan(context, context.BuildKey);

            try
            {
                plan.BuildUp(context);
                Assert.Fail("failure expected");
            }
            catch (Exception e)
            {
                Assert.AreSame(exception, e);

                var operation = (ConstructorArgumentResolveOperation)context.CurrentOperation;
                Assert.IsNotNull(operation);

                Assert.AreSame(typeof(ConstructorInjectionTestClass), operation.TypeBeingConstructed);
                Assert.AreEqual("parameter", operation.ParameterName);
            }
        }

        public class TestSingleArgumentConstructorSelectorPolicy<T> : IConstructorSelectorPolicy
        {
            private IResolverPolicy parameterResolverPolicy;

            public TestSingleArgumentConstructorSelectorPolicy(IResolverPolicy parameterResolverPolicy)
            {
                this.parameterResolverPolicy = parameterResolverPolicy;
            }

            public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPoliciesDestination)
            {
                var selectedConstructor = new SelectedConstructor(typeof(T).GetConstructor(new[] { typeof(object) }));
                selectedConstructor.AddParameterResolver(this.parameterResolverPolicy);

                return selectedConstructor;
            }
        }

        private MockBuilderContext GetContext()
        {
            StagedStrategyChain<BuilderStage> chain = new StagedStrategyChain<BuilderStage>();
            chain.Add(new DynamicMethodConstructorStrategy(), BuilderStage.Creation);

            DynamicMethodBuildPlanCreatorPolicy policy =
                new DynamicMethodBuildPlanCreatorPolicy(chain);

            MockBuilderContext context = new MockBuilderContext();

            context.Strategies.Add(new LifetimeStrategy());

            context.PersistentPolicies.SetDefault<IConstructorSelectorPolicy>(
                new ConstructorSelectorPolicy<InjectionConstructorAttribute>());
            context.PersistentPolicies.SetDefault<IBuildPlanCreatorPolicy>(policy);

            return context;
        }

        private IBuildPlanCreatorPolicy GetPlanCreator(IBuilderContext context)
        {
            return context.Policies.Get<IBuildPlanCreatorPolicy>(null);
        }

        public class ConstructorInjectionTestClass
        {
            public ConstructorInjectionTestClass(object parameter)
            {
            }
        }

        public class ThrowingConstructorInjectionTestClass
        {
            public static Exception ConstructorException = new ArgumentException();

            public ThrowingConstructorInjectionTestClass()
            {
                throw ConstructorException;
            }
        }

        public class NoPublicConstructorInjectionTestClass
        {
            private NoPublicConstructorInjectionTestClass() { }

            public static NoPublicConstructorInjectionTestClass CreateInstance()
            {
                return new NoPublicConstructorInjectionTestClass();
            }
        }
    }
}