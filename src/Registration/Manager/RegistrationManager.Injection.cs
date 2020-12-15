﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Storage;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager : IEnumerable, 
                                                        ISequenceSegment<InjectionMethodBase<ConstructorInfo>?>,
                                                        ISequenceSegment<InjectionMemberInfo<FieldInfo>?>,
                                                        ISequenceSegment<InjectionMemberInfo<PropertyInfo>?>,
                                                        ISequenceSegment<InjectionMethodBase<MethodInfo>?>,
                                                        ISequenceSegment<InjectionMember?>
    {
        #region Injection Constructor

        public InjectionMethodBase<ConstructorInfo>? Constructor { get; private set; }

        InjectionMethodBase<ConstructorInfo>? ISequenceSegment<InjectionMethodBase<ConstructorInfo>?>.Next 
            => Constructor;

        int ISequenceSegment<InjectionMethodBase<ConstructorInfo>?>.Length 
            => Constructor?.Length ?? 0;

        #endregion


        #region Injection Fields

        public InjectionMemberInfo<FieldInfo>? Fields { get; private set; }

        InjectionMemberInfo<FieldInfo>? ISequenceSegment<InjectionMemberInfo<FieldInfo>?>.Next 
            => Fields;

        int ISequenceSegment<InjectionMemberInfo<FieldInfo>?>.Length 
            => Fields?.Length ?? 0;

        #endregion


        #region Injection Properties

        public InjectionMemberInfo<PropertyInfo>? Properties { get; private set; }

        InjectionMemberInfo<PropertyInfo>? ISequenceSegment<InjectionMemberInfo<PropertyInfo>?>.Next 
            => Properties;

        int ISequenceSegment<InjectionMemberInfo<PropertyInfo>?>.Length 
            => Properties?.Length ?? 0;

        #endregion


        #region Injection Methods

        public InjectionMethodBase<MethodInfo>? Methods { get; private set; }

        InjectionMethodBase<MethodInfo>? ISequenceSegment<InjectionMethodBase<MethodInfo>?>.Next 
            => Methods;

        int ISequenceSegment<InjectionMethodBase<MethodInfo>?>.Length 
            => Methods?.Length ?? 0;

        #endregion


        #region Other

        public InjectionMember? Other { get; private set; }

        InjectionMember? ISequenceSegment<InjectionMember?>.Next 
            => Other;

        int ISequenceSegment<InjectionMember?>.Length 
            => Other?.Length ?? 0;

        #endregion


        #region Initializers

        public void Add(InjectionMember member)
        {
            switch (member)
            {
                case InjectionMethodBase<ConstructorInfo> ctor:
                    ctor.Next = Constructor;
                    ctor.Length = (Constructor?.Length ?? 0) + 1;
                    Constructor = ctor;
                    break;

                case InjectionMemberInfo<FieldInfo> field:
                    field.Next = Fields;
                    field.Length = (Fields?.Length ?? 0) + 1;
                    Fields = field;
                    break;

                case InjectionMemberInfo<PropertyInfo> property:
                    property.Next = Properties;
                    property.Length = (Properties?.Length ?? 0) + 1;
                    Properties = property;
                    break;

                case InjectionMethodBase<MethodInfo> method:
                    method.Next = Methods;
                    method.Length = (Methods?.Length ?? 0) + 1;
                    Methods = method;
                    break;

                default:
                    member.Next = Other;
                    member.Length = (Other?.Length ?? 0) + 1;
                    Other = member;
                    break;
            }

            RequireBuild |= member.BuildRequired;
        }

        public void Add(IEnumerable<InjectionMember> members)
        {
            foreach (var member in members) Add(member);
        }

        #endregion


        #region IEnumerable

        public IEnumerator<InjectionMember> GetEnumerator()
        {
            // Start with constructor (Only one constructor)
            if (null != Constructor) yield return Constructor;

            // Fields
            for (InjectionMember? member = Fields; null != member; member = member.Next)
                yield return member;

            // Properties
            for (InjectionMember? member = Properties; null != member; member = member.Next)
                yield return member;

            // Methods
            for (InjectionMember? member = Methods; null != member; member = member.Next)
                yield return member;

            // Other
            for (InjectionMember? member = Other; null != member; member = member.Next)
                yield return member;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}