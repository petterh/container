﻿using System;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        private object? ResolveRegistration(ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            // Create context
            var context = parent.CreateContext(this, ref contract, manager);

            // Double lock check and create pipeline
            if (manager.Pipeline is null) lock (manager) if (manager.Pipeline is null)
                manager.Pipeline = BuildPipeline(ref context);

            // Resolve
            context.Target = manager.Pipeline!(ref context);

            // Save result
            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);

            return context.Target;
        }

        private object? ResolveRegistration(ref PipelineContext context)
        {
            var manager = context.Registration!;

            context.Target = manager.GetValue(_scope);

            if (!ReferenceEquals(RegistrationManager.NoValue, context.Target)) return context.Target;
            else context.Target = null; // TODO: context.Target = null

            // Double lock check and create pipeline
            if (manager.Pipeline is null) lock (manager) if (manager.Pipeline is null)
                manager.Pipeline = BuildPipeline(ref context);

            // Resolve
            using (var action = context.Start(manager.Data!))
            {
                context.Target = manager.Pipeline!(ref context);
            }

            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);

            return context.Target;
        }

        private void BuildUpRegistration(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Check if pipeline has been created already
            if (manager.Pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (manager)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (manager.Pipeline is null)
                    {
                        using var action = context.Start(manager);

                        switch (manager.Category)
                        {
                            case RegistrationCategory.Type:

                                // Check for Type Mapping
                                var registration = context.Registration;
                                if (null != registration && !registration.RequireBuild && context.Contract.Type != registration.Type)
                                {
                                    var contract = new Contract(registration.Type!, context.Contract.Name);

                                    manager.Pipeline = (ref PipelineContext c) =>
                                    {
                                        var stack = contract;
                                        var local = c.CreateContext(ref stack);

                                        c.Target = local.Resolve();

                                        return c.Target;
                                    };
                                }
                                else
                                { 
                                    manager.Pipeline = _policies.BuildTypePipeline(context.Contract.Type);
                                }

                                break;

                            case RegistrationCategory.Factory:
                                manager.Pipeline = _policies.BuildFactoryPipeline(context.Contract.Type);
                                break;

                            case RegistrationCategory.Instance:
                                manager.Pipeline = _policies.BuildInstancePipeline(context.Contract.Type);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }

            // Resolve
            using (var action = context.Start(manager.Data!))
            {
                manager.Pipeline!(ref context);
            }
        }
    }
}