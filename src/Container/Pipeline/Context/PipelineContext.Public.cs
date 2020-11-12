using System;
using System.Runtime.CompilerServices;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region Resolution

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Resolve()
        {
            Target = Container.Resolve(ref this);
            return Target;
        }

        public object? Resolve(Type type, string? name)
        {
            var contract = new Contract(type, name);

            Target = Container.ResolveContract(ref contract, ref this);

            return Target;
        }

        #endregion


        #region Indirection

        public bool IsFaulted
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ErrorInfo>(_error.ToPointer()).IsFaulted;
                }
            }
        }

        public readonly ref Contract Contract
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
                }
            }
        }

        public readonly ref ErrorInfo ErrorInfo
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ErrorInfo>(_error.ToPointer());
                }
            }
        }

        private readonly ref RequestInfo RequestInfo
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<RequestInfo>(_request.ToPointer());
                }
            }
        }

        public readonly ref PipelineContext Parent
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<PipelineContext>(_parent.ToPointer());
                }
            }
        }

        public readonly ResolverOverride[] Overrides
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).Overrides;
                }
            }
        }

        public LifetimeManager? LifetimeManager => Registration as LifetimeManager;

        public Defaults Defaults => Container._policies;

        #endregion


        #region Public Methods

        public object? PerResolve
        {
            set
            {
                Target = value;
                RequestInfo.PerResolve = new PerResolveOverride(in Contract, value);
            }
        }

        public object Error(string error)
        {
            unsafe
            {
                ref var info = ref Unsafe.AsRef<ErrorInfo>(_error.ToPointer());

                info.IsFaulted = true;
                info.Message = error;
            }

            return error;
        }

        public void Capture(Exception exception)
        {
            unsafe
            {
                Unsafe.AsRef<ErrorInfo>(_error.ToPointer()).Capture(exception);
            }
        }

        public object? GetValueRecursively<TInfo>(TInfo info, object? value)
        {
            return value switch
            {
                ResolveDelegate<PipelineContext> resolver => GetValueRecursively(info, resolver(ref this)),

                IResolve iResolve                         => GetValueRecursively(info, iResolve.Resolve(ref this)),

                IResolverFactory<TInfo> infoFactory       => GetValueRecursively(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                       .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => GetValueRecursively(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                       .Invoke(ref this)),
                _ => value,
            };
        }


        #endregion


        #region Telemetry

        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class
            => new PipelineAction<TAction>(ref this, action);

        #endregion


        #region Child Context

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(UnityContainer container, ref Contract contract, RegistrationManager manager)
            => new PipelineContext(container, ref contract, manager, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract, RegistrationManager manager)
            => new PipelineContext(Container, ref contract, manager, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(UnityContainer container, ref Contract contract)
            => new PipelineContext(container, ref contract, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error)
            => new PipelineContext(ref contract, ref error, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract)
            => new PipelineContext(ref contract, ref this);

        #endregion
    }
}
