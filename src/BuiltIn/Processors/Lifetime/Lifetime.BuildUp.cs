﻿using Unity.Lifetime;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor
    {
        public override void PreBuildUp(ref PipelineContext pipeline)
        {
            //var lifetime = pipeline.Manager;

            //// Resolution context
            //ref var context = ref pipeline.Context;

            ////if (null == lifetime || lifetime is PerResolveLifetimeManager)
            ////    lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            ////if (lifetime is SynchronizedLifetimeManager recoveryPolicy)
            ////    context.RequiresRecovery = recoveryPolicy;

            //var existing = lifetime.GetValue(context.Disposables);
            //if (!ReferenceEquals(RegistrationManager.NoValue, existing))
            //{
            //    context.Existing = existing;
            //}
        }


        public override void PostBuildUp(ref PipelineContext pipeline)
        {
            //LifetimeManager lifetime = (LifetimeManager)context.Manager!;

            //if (null == lifetime || lifetime is PerResolveLifetimeManager)
            //    lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            //if (!ReferenceEquals(RegistrationManager.NoValue, context.Existing))
            //    lifetime?.SetValue(context.Existing, context.Disposables);
        }
    }
}
