﻿using System;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData FromContainer<TContext>(ref TContext context, ref ImportInfo<TDependency> import)
            where TContext : IBuilderContext
        {
            ErrorInfo error   = default;
            Contract contract = new Contract(import.Contract.Type, import.Contract.Name);  // TODO: Optimize
            
            var value = import.AllowDefault
                ? context.FromContract(ref contract, ref error)
                : context.Resolve(import.Contract.Type, import.Contract.Name);

            if (error.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return GetDefault(ref import);
            }

            return new ImportData(value, ImportType.Value);
        }


        protected ImportData FromPipeline<TContext>(ref TContext context, ref ImportInfo<TDependency> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext
        {
            var contract = new Contract(import.Contract.Type, import.Contract.Name);

            return new ImportData(context.FromPipeline(ref contract, pipeline), ImportType.Value);
        }

        protected ImportData FromUnknown<TContext>(ref TContext context, ref ImportInfo<TDependency> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportDescriptionProvider<TDependency> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        return FromPipeline(ref context, ref import, iResolve.Resolve);

                    case ResolveDelegate<TContext> resolver:
                        return FromPipeline(ref context, ref import, resolver);

                    case IResolverFactory<Type> typeFactory:
                        return FromPipeline(ref context, ref import, typeFactory.GetResolver<TContext>(import.MemberType));

                    case PipelineFactory<TContext> factory:
                        return FromPipeline(ref context, ref import, factory(ref context));

                    case Type target when typeof(Type) != import.MemberType:
                        import.Contract = new Contract(target);
                        import.AllowDefault = false;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref import);

                    default:
                        return new ImportData(data, ImportType.Value);
                }
            }
            while (ImportType.Unknown == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None => FromContainer(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }
    }
}
