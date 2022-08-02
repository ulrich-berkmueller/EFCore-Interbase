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

using Microsoft.EntityFrameworkCore.Storage;

namespace SK.EntityFrameworkCore.Interbase.Storage.Internal;

public class InterbaseBoolTypeMapping : BoolTypeMapping
{
	public bool IsUsedAsSingleConstantConditionInWherePart { get; set; } = false;

	public InterbaseBoolTypeMapping()
		: base("BOOLEAN", System.Data.DbType.Boolean)
	{ }

	protected InterbaseBoolTypeMapping(RelationalTypeMappingParameters parameters)
		: base(parameters)
	{ }

	protected override string GenerateNonNullSqlLiteral(object value)
	{
		if (IsUsedAsSingleConstantConditionInWherePart)
		{
			return (bool)value ? "1=1" : "1=0";
		}
		else
		{
		return (bool)value ? "TRUE" : "FALSE";
	}
	}

	protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
		=> new InterbaseBoolTypeMapping(parameters)
			{
				IsUsedAsSingleConstantConditionInWherePart = IsUsedAsSingleConstantConditionInWherePart
			};
}
