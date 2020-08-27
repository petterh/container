﻿using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// that deal in explicit types.
    /// </summary>
    public abstract class ParameterBase : ParameterValue
    {
        #region Fields

        private readonly Type? _type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="ParameterBase"/> that exposes
        /// information about the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        protected ParameterBase(Type? type = null)
        {
            _type = type;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type? ParameterType => _type;

        #endregion


        #region Overrides

        public override MatchRank MatchTo(Type type)
        {
            return null == _type 
                ? MatchRank.ExactMatch
                : _type.MatchTo(type);
        }

        public override MatchRank MatchTo(ParameterInfo parameter) => 
            MatchTo(parameter.ParameterType);

        #endregion


        #region Implementation

        protected bool IsInvalidParameterType
        {
            get
            {
                return null == ParameterType ||
                    ParameterType.IsGenericType() && ParameterType.ContainsGenericParameters() ||
                    ParameterType.IsArray         && ParameterType.GetElementType()!.IsGenericParameter ||
                    ParameterType.IsGenericParameter;
            }
        }


        #endregion
    }
}
