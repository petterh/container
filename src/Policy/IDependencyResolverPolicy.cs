﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that is used at build plan execution time
    /// to resolve a dependent value.
    /// </summary>
    public interface IDependencyResolverPolicy : IBuilderPolicy
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        object Resolve(IBuilderContext context);
    }
}
