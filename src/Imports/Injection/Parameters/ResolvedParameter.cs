﻿using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// Instances of this class instruct the container to inject corresponding
    /// parameters with values imported from this container
    /// </summary>
    /// <remarks>
    /// When the container fails to inject specified parameters with required
    /// import, the entire resolution request fails and error is generated
    /// </remarks>
    [DebuggerDisplay("ResolvedParameter: Type={ParameterType?.Name ?? \"Any\"} Name={_name ?? \"null\"}")]
    public class ResolvedParameter : ParameterBase, 
                                     IResolverFactory<Type>,
                                     IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly string? _name;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to inject parameter with value resolved 
        /// from the container
        /// </summary>
        /// <remarks>
        /// The parameter is injected with value imported from the container. 
        /// The <see cref="Type"/> of imported contract is the <see cref="Type"/>
        /// of the parameter and no name.
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        public ResolvedParameter()
        {
        }

        /// <summary>
        /// Configures the container to inject parameter with specified <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of this parameter.</param>
        public ResolvedParameter(Type contractType)
            : base(contractType)
        {
        }

        /// <summary>
        /// Configures the container to inject parameter with <see cref="Contract"/> with 
        /// <see cref="Type"/> being the <see cref="Type"/> of the parameter and specified
        /// name.
        /// </summary>
        /// <remarks>
        /// The parameter is injected with value imported from the container. The <see cref="Type"/> of 
        /// imported contract is the <see cref="Type"/> of the parameter and name of the 
        /// <see cref="Contract"/> is provided in <paramref name="contractName"/>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public ResolvedParameter(string contractName) 
            => _name = contractName;

        /// <summary>
        /// Configures the container to inject parameter with specified <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public ResolvedParameter(Type contractType, string? contractName)
            : base(contractType) => _name = contractName;

        #endregion


        #region IResolverFactory

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            if (IsInvalidParameterType)
            {
                return (ref TContext c) => c.Resolve(type, _name);
            }

            return (ref TContext c) => c.Resolve(ParameterType!, _name);
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info) 
            where TContext : IResolveContext
        {
            if (IsInvalidParameterType)
            {
                var type = info.ParameterType;
                return (ref TContext c) => c.Resolve(type, _name);
            }

            return (ref TContext c) => c.Resolve(ParameterType!, _name);
        }

        #endregion


        #region Overrides

        public override string ToString()
        {
            return $"ResolvedParameter: Type={ParameterType?.Name ?? "Any"} Name={_name ?? "null"}";
        }

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="ResolvedParameter"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of injected <see cref="Contract"/></typeparam>
    public class ResolvedParameter<T> : ResolvedParameter
    {
        /// <inheritdoc/>
        public ResolvedParameter() : base(typeof(T))
        {
        }

        /// <inheritdoc/>
        public ResolvedParameter(string name) : base(typeof(T), name)
        {
        }
    }

    #endregion
}