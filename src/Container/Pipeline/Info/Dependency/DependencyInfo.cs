﻿using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public struct DependencyInfo<TInfo>
    {
        private readonly IntPtr _parent;

        public bool AllowDefault;
        public TInfo Info;
        public Contract Contract;
        public ImportAttribute? Import;

        public DependencyInfo(ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            Info = default!;
            Import = default;
            Contract = default;
            AllowDefault = default;
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

        /// <summary>
        /// Returns override matching this dependency 
        /// </summary>
        /// or <see cref="PropertyInfo"/></typeparam>
        /// <returns><see cref="ResolverOverride"/> object or null</returns>
        public ResolverOverride? GetOverride()
        {
            var overrides = Parent.Overrides;
            if (null == overrides) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank candidateRank = MatchRank.NoMatch;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                var rank = Unsafe.As<IMatchContract<TInfo>>(@override)
                                 .Match(Info, in Contract);

                if (MatchRank.ExactMatch == rank) return @override;

                if (rank > candidateRank)
                {
                    candidateRank = rank;
                    candidateOverride = @override;
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
                return candidateOverride;

            return null;
        }
    }
}
