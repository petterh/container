using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InjectionMember<TMemberInfo, TData>? OfType<TMemberInfo, TData>()
            where TMemberInfo : MemberInfo where TData : class
            => (Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>)?.Next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> OfType<T>()
            => Registration?.OfType<T>() ?? Enumerable.Empty<T>();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Get(Type type)
            => Registration?.Get(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Type type, object policy)
            => Registration?.Set(type, policy);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Type type)
            => Registration?.Clear(type);

    }
}