/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using SK.EntityFrameworkCore.Interbase.Metadata;
using SK.EntityFrameworkCore.Interbase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class InterbasePropertyBuilderExtensions
{
	public static PropertyBuilder UseIdentityColumn(this PropertyBuilder propertyBuilder)
	{
		var property = propertyBuilder.Metadata;
		property.SetValueGenerationStrategy(InterbaseValueGenerationStrategy.IdentityColumn);
		return propertyBuilder;
	}

	public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
		=> (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder);

	public static PropertyBuilder UseSequenceTrigger(this PropertyBuilder propertyBuilder)
	{
		var property = propertyBuilder.Metadata;
		property.SetValueGenerationStrategy(InterbaseValueGenerationStrategy.SequenceTrigger);
		return propertyBuilder;
	}

	public static PropertyBuilder<TProperty> UseSequenceTrigger<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
		=> (PropertyBuilder<TProperty>)UseSequenceTrigger((PropertyBuilder)propertyBuilder);

	public static IConventionPropertyBuilder HasValueGenerationStrategy(this IConventionPropertyBuilder propertyBuilder, InterbaseValueGenerationStrategy? valueGenerationStrategy, bool fromDataAnnotation = false)
	{
		if (propertyBuilder.CanSetAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
		{
			propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
			if (valueGenerationStrategy != InterbaseValueGenerationStrategy.IdentityColumn)
			{
			}
			if (valueGenerationStrategy != InterbaseValueGenerationStrategy.SequenceTrigger)
			{
			}
			return propertyBuilder;
		}
		return null;
	}
}
