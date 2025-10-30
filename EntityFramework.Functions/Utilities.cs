namespace EntityFramework.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
  	using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    internal static class StringExtensions
    {
        internal static bool EqualsOrdinal
                (this string a, string b) => string.Equals(a, b, StringComparison.Ordinal);
    }

    internal static class EnumerableExtensions
    {
        internal static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> next)
        {
            foreach (TSource value in source)
            {
                next(value);
            }
        }
    }

    internal static class EdmPropertyExtensions
    {
        internal static EdmProperty Clone(this EdmProperty property, DbModel model)
        {
            TypeUsage typeUsage;

            if (property.IsEnumType)
            {
                var underlyingPrimitiveType = PrimitiveType.GetEdmPrimitiveType(property.UnderlyingPrimitiveType.PrimitiveTypeKind);
                typeUsage = model.ProviderManifest.GetStoreType(TypeUsage.Create(underlyingPrimitiveType, property.TypeUsage.Facets)); // TypeUsage.Create(underlyingPrimitiveType, property.TypeUsage.Facets);
            }
            else if (property.IsPrimitiveType)
            {
                var primitiveType = PrimitiveType.GetEdmPrimitiveType(property.PrimitiveType.PrimitiveTypeKind);
                typeUsage = model.ProviderManifest.GetStoreType(property.TypeUsage); // TypeUsage.Create(primitiveType, property.TypeUsage.Facets);
            }
            else
            {
                typeUsage = property.TypeUsage;
            }

            var clone = EdmProperty.Create(property.Name, typeUsage);

            if (clone.Nullable != property.Nullable) clone.Nullable = property.Nullable;
            if (clone.CollectionKind != property.CollectionKind) clone.CollectionKind = property.CollectionKind;
            if (clone.ConcurrencyMode != property.ConcurrencyMode) clone.ConcurrencyMode = property.ConcurrencyMode;
            if (clone.IsFixedLength != property.IsFixedLength) clone.IsFixedLength = property.IsFixedLength;
            if (clone.IsMaxLength != property.IsMaxLength) clone.IsMaxLength = property.IsMaxLength;
            if (clone.IsUnicode != property.IsUnicode) clone.IsUnicode = property.IsUnicode;
            if (clone.MaxLength != property.MaxLength) clone.MaxLength = property.MaxLength;
            if (clone.Precision != property.Precision) clone.Precision = property.Precision;
            if (clone.Scale != property.Scale) clone.Scale = property.Scale;
            if (clone.StoreGeneratedPattern != property.StoreGeneratedPattern) clone.StoreGeneratedPattern = property.StoreGeneratedPattern;
            clone.SetMetadataProperties(property
                .MetadataProperties
                .Where(metadataProerty => !clone
                    .MetadataProperties
                    .Any(cloneMetadataProperty => cloneMetadataProperty.Name.EqualsOrdinal(metadataProerty.Name))));
            return clone;
        }
    }

#if NET40
    internal static class CustomAttributeExtensions
    {
        internal static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute =>
            (T)element.GetCustomAttribute(typeof(T));

        internal static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType) =>
            Attribute.GetCustomAttribute(element, attributeType);

        internal static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute =>
            (T)element.GetCustomAttribute(typeof(T));

        internal static Attribute GetCustomAttribute(this ParameterInfo element, Type attributeType) =>
            Attribute.GetCustomAttribute(element, attributeType);

        internal static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute =>
            (IEnumerable<T>)element.GetCustomAttributes(typeof(T));

        internal static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType) =>
            Attribute.GetCustomAttributes(element, attributeType);
    }
#endif
}