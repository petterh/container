﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    internal static class InternalExtensions
    {
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();

        public static bool CanResolve(this UnityContainer container,  Type type, string? name)
        {
#if NETSTANDARD1_6 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
#else
            var info = type;
#endif
            if (info.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return container.IsRegistered(type, name) || container.CanResolve(type.GetElementType()!, name);
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract
#if NETSTANDARD1_6 || NETCOREAPP1_0
                    || !info.DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#else
                    || !type.GetTypeInfo().DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#endif
                    return container.IsRegistered(type, name);

                return true;
            }

            // Can resolve if IEnumerable or factory is registered
            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || container.IsRegistered(genericType, name))
                {
                    return true;
                }
            }

            // Check if Type is registered
            return container.IsRegistered(type, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type) where T : Delegate
            => (T)method.CreateDelegate(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type, object? target) where T : Delegate
            => (T)method.CreateDelegate(type, target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method)
            => (ResolveDelegate<PipelineContext>)method.CreateDelegate(typeof(ResolveDelegate<PipelineContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, object? target)
            => (ResolveDelegate<PipelineContext>)method.CreateDelegate(typeof(ResolveDelegate<PipelineContext>), target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<PipelineContext> CreatePipeline(this MethodInfo method, Type type, object? target)
            => (ResolveDelegate<PipelineContext>)method.MakeGenericMethod(type)
                                                       .CreateDelegate(typeof(ResolveDelegate<PipelineContext>), target);
    }
}
