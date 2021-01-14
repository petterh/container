﻿using System;
using System.ComponentModel.Composition;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineResolved(ref BuilderContext context)
        {
            switch (context.Registration?.CreationPolicy)
            {
                case CreationPolicy.Any:
                    break;

                case CreationPolicy.Shared:
                    return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;

                case CreationPolicy.NonShared:
                    break;
            }

            var chain = ((Policies<BuilderContext>)context.Policies)!.TypeChain;
            var builder = new PipelineBuilder<BuilderContext>(chain);

            return builder.Build(ref context) ?? UnityContainer.DummyPipeline;
        }
    }
}
