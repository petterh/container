﻿using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Chains

        public StagedChain<BuildStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Pipeline Builder
        
        public ProducerFactory BuildPipeline
            => (ProducerFactory)Data[BUILD_PIPELINE].Value!;


        #endregion


        #region Resolution

        public RegistrationProducerDelegate ResolveContract
            => (RegistrationProducerDelegate)Data[RESOLVE_CONTRACT].Value!;

        public ResolveUnregisteredDelegate ResolveUnregistered
            => (ResolveUnregisteredDelegate)Data[RESOLVE_UNKNOWN].Value!;

        public ResolveMappedDelegate ResolveMapped
            => (ResolveMappedDelegate) Data[RESOLVE_MAPPED].Value!;

        public ResolveArrayDelegate ResolveArray
            => (ResolveArrayDelegate)Data[RESOLVE_ARRAY].Value!;

        #endregion


        #region Activators

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Type"/> registration
        /// </summary>
        public ServiceProducer TypePipeline
            => (ServiceProducer)Data[PIPELINE_TYPE].Value!;

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Instance"/> registration
        /// </summary>
        public ResolveDelegate<ResolutionContext> InstancePipeline
            => (ResolveDelegate<ResolutionContext>)Data[PIPELINE_INSTANCE].Value!;

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Factory"/> registration
        /// </summary>
        public ResolveDelegate<ResolutionContext> FactoryPipeline
            => (ResolveDelegate<ResolutionContext>)Data[PIPELINE_FACTORY].Value!;

        #endregion


        #region Factories

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInWhile"/> lifetime
        /// </summary>
        public SingletonFactoryDelegate SingletonPipelineFactory
            => (SingletonFactoryDelegate)Data[FACTORY_SINGLETON].Value!;

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInWhile"/> lifetime
        /// </summary>
        public BalancedFactoryDelegate BalancedPipelineFactory
            => (BalancedFactoryDelegate)Data[FACTORY_BALANCED].Value!;

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.EveryTime"/> lifetime
        /// </summary>
        public OptimizedFactoryDelegate OptimizedPipelineFactory
            => (OptimizedFactoryDelegate)Data[FACTORY_OPTIMIZED].Value!;

        #endregion
    }
}
