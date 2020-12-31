﻿using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity;

namespace Unity
{
    public partial class UnityContainer
    {
        internal object? Resolve(ref PipelineContext context)
        {
            var container = this;
            Contract generic = default;

            do
            {
                // Try to get registration
                context.Registration ??= container.Scope.Get(in context.Contract);
                if (null != context.Registration)
                {
                    var value = Unsafe.As<LifetimeManager>(context.Registration).GetValue(Scope);
                    if (value.IsValue())
                    {
                        context.Target = value;
                        return value;
                    }

                    var source = ImportSource.Local == context.Registration.Source 
                               ? this 
                               : container;
                    
                    using var scope = context.WithContainer(source);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.Contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    var source = ImportSource.Local == context.Registration.Source
                               ? this 
                               : container;
                    
                    using var scope = context.WithContainer(source);
                    
                    return GenericRegistration(generic.Type!, ref context);
                }
            }
            while (null != (container = container.Parent!));

            return context.Contract.Type.IsGenericType 
                ? GenericUnregistered(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? ResolveArray(ref context)
                    : Policies.ResolveUnregistered(ref context);
        }


        /// <summary>
        /// Resolve unregistered from Contract
        /// </summary>
        private object? ResolveUnregistered(ref Contract contract, ref RequestInfo request)
        {
            PipelineContext context;
            RegistrationManager? manager;
            Contract generic = default;

            // Skip to parent if non generic
            if (contract.Type.IsGenericType)
            {
                // Fill the Generic Type Definition
                generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = Scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = new PipelineContext(this, ref contract, manager, ref request);
                    return GenericRegistration(generic.Type!, ref context);
                }
            }

            var container = this;
            while (null != (container = container.Parent!))
            {
                // Try to get registration
                manager = container.Scope.Get(in contract);
                if (null != manager)
                {
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(Scope);
                    if (value.IsValue()) return value;

                    var scope = ImportSource.Local == manager.Source ? this : container;
                    context = new PipelineContext(scope, ref contract, manager, ref request);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!contract.Type.IsGenericType) continue;

                // Check if generic factory is registered
                if (null != (manager = container.Scope.GetBoundGeneric(in contract, in generic)))
                {
                    var scope = ImportSource.Local == manager.Source ? this : container;
                    context = new PipelineContext(scope, ref contract, manager, ref request);

                    return GenericRegistration(generic.Type!, ref context);
                }
            }

            context = new PipelineContext(this, ref contract, ref request);

            if (Policies.TryGet(contract.Type, out ResolveDelegate<PipelineContext>? pipeline))
                return pipeline!(ref context);

            return contract.Type.IsGenericType
                ? GenericUnregistered(ref generic, ref context)
                : contract.Type.IsArray
                    ? ResolveArray(ref context)
                    : Policies.ResolveUnregistered(ref context);
        }

    }
}
