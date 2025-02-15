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

using System;
using SK.EntityFrameworkCore.Interbase.Metadata;
using SK.EntityFrameworkCore.Interbase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

public static class InterbasePropertyExtensions
{
	public static InterbaseValueGenerationStrategy GetValueGenerationStrategy(this IProperty property)
	{
		var annotation = property[InterbaseAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (InterbaseValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return InterbaseValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == InterbaseValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return InterbaseValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == InterbaseValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			return InterbaseValueGenerationStrategy.IdentityColumn;
		}

		return InterbaseValueGenerationStrategy.None;
	}

	public static InterbaseValueGenerationStrategy GetValueGenerationStrategy(this IMutableProperty property)
	{
		var annotation = property[InterbaseAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (InterbaseValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return InterbaseValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == InterbaseValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return InterbaseValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == InterbaseValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			return InterbaseValueGenerationStrategy.IdentityColumn;
		}

		return InterbaseValueGenerationStrategy.None;
	}

	public static InterbaseValueGenerationStrategy GetValueGenerationStrategy(this IConventionProperty property)
	{
		var annotation = property[InterbaseAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (InterbaseValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return InterbaseValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == InterbaseValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return InterbaseValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == InterbaseValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			return InterbaseValueGenerationStrategy.IdentityColumn;
		}

		return InterbaseValueGenerationStrategy.None;
	}

	public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(this IConventionProperty property)
		=> property.FindAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

	public static void SetValueGenerationStrategy(this IMutableProperty property, InterbaseValueGenerationStrategy? value)
	{
		CheckValueGenerationStrategy(property, value);
		property.SetOrRemoveAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, value);
	}

	public static void SetValueGenerationStrategy(this IConventionProperty property, InterbaseValueGenerationStrategy? value, bool fromDataAnnotation = false)
	{
		CheckValueGenerationStrategy(property, value);
		property.SetOrRemoveAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);
	}

	static void CheckValueGenerationStrategy(IReadOnlyPropertyBase property, InterbaseValueGenerationStrategy? value)
	{
		if (value != null)
		{
			if (value == InterbaseValueGenerationStrategy.IdentityColumn && !IsCompatibleIdentityColumn(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(InterbaseValueGenerationStrategy.IdentityColumn)} for '{property.Name}'.");
			}
			if (value == InterbaseValueGenerationStrategy.SequenceTrigger && !IsCompatibleSequenceTrigger(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(InterbaseValueGenerationStrategy.SequenceTrigger)} for '{property.Name}'.");
			}
		}
	}

	static bool IsCompatibleIdentityColumn(IReadOnlyPropertyBase property)
		=> property.ClrType.IsInteger() || property.ClrType == typeof(decimal);

	static bool IsCompatibleSequenceTrigger(IReadOnlyPropertyBase property)
		=> true;
}
