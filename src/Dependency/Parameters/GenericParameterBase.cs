﻿using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for generic type parameters.
    /// </summary>
    public abstract class GenericParameterBase : ParameterValue,
                                                 IResolverFactory<Type>,
                                                 IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly bool    _isArray;
        private readonly string? _name;
        private readonly string  _genericParameterName;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        protected GenericParameterBase(string genericParameterName)
            : this(genericParameterName, null)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="resolutionName">Registration name to use when looking up in the container.</param>
        protected GenericParameterBase(string genericParameterName, string? resolutionName)
        {
            if (null == genericParameterName) throw new ArgumentNullException(nameof(genericParameterName));

            if (genericParameterName.EndsWith("[]", StringComparison.Ordinal) ||
                genericParameterName.EndsWith("()", StringComparison.Ordinal))
            {
                _genericParameterName = genericParameterName.Replace("[]", string.Empty).Replace("()", string.Empty);
                _isArray = true;
            }
            else
            {
                _genericParameterName = genericParameterName;
                _isArray = false;
            }
            _name = resolutionName;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public virtual string ParameterTypeName => _genericParameterName;

        #endregion


        #region  IMatch

        public override MatchRank MatchTo(ParameterInfo parameter)
        {
            if (!parameter.Member.DeclaringType!.IsGenericType())
                return MatchRank.NoMatch;

            var definition = parameter.Member.DeclaringType!.GetGenericTypeDefinition();
            var type = ((MethodBase)parameter.Member).GetMemberFromInfo(definition)!
                                                     .GetParameters()[parameter.Position]
                                                     .ParameterType;
            return MatchTo(type);
        }

        public override MatchRank MatchTo(Type type)
        {
            if (false == _isArray)
                return type.IsGenericParameter && type.Name == _genericParameterName
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch; 

            if (!type.IsArray) return MatchRank.NoMatch;

            return _genericParameterName.Equals(type.GetElementType()!.Name) 
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion


        #region IResolverFactory

        public virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext => GetResolver<TContext>(type, _name);

        public virtual ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext => GetResolver<TContext>(info.ParameterType, _name);

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type, string? name)
            where TContext : IResolveContext => (ref TContext context) => context.Resolve(type, name);

        #endregion
    }
}
