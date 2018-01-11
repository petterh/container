﻿using System;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Registration;
using Unity.Strategy;

namespace Unity.ObjectBuilder.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that will look for a build plan
    /// in the current context. If it exists, it invokes it, otherwise
    /// it creates one and stores it for later, and invokes it.
    /// </summary>
    public class BuildPlanStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override object PreBuildUp(IBuilderContext context)
        {
            var plan = context.Policies.Get<IBuildPlanPolicy>(context.OriginalBuildKey, out var buildPlanLocation);
            if (plan == null || plan is OverriddenBuildPlanMarkerPolicy)
            {
                var planCreator = context.Policies.Get<IBuildPlanCreatorPolicy>(context.BuildKey, out var creatorLocation);
                if (planCreator != null)
                {
                    plan = planCreator.CreatePlan(context, context.BuildKey);
                    (buildPlanLocation ?? creatorLocation).Set(plan, context.OriginalBuildKey);
                }
            }

            plan?.BuildUp(context);
            return null;
        }
    }
}