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
using System.Collections.Generic;
using System.Linq;
using SK.EntityFrameworkCore.Interbase.Utilities;
using Microsoft.EntityFrameworkCore.Query;

namespace SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;

public class InterbaseMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
{
	static readonly List<Type> Translators = TranslatorsHelper.GetTranslators<IMethodCallTranslator>().ToList();

	public InterbaseMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
		: base(dependencies)
	{
		AddTranslators(Translators.Select(t => (IMethodCallTranslator)Activator.CreateInstance(t, dependencies.SqlExpressionFactory)));
	}
}
