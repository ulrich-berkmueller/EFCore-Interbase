/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Threading.Tasks;
using FirebirdSql.EntityFrameworkCore.Firebird.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;

namespace FirebirdSql.EntityFrameworkCore.Firebird.FunctionalTests.Query
{
	public class NullSemanticsQueryFbTest : NullSemanticsQueryTestBase<NullSemanticsQueryFbFixture>
	{
		public NullSemanticsQueryFbTest(NullSemanticsQueryFbFixture fixture)
			: base(fixture)
		{ }

		protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
		{
			var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
			if (useRelationalNulls)
			{
				new FbDbContextOptionsBuilder(options).UseRelationalNulls();
			}
			var context = new NullSemanticsContext(options.Options);
			context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
			return context;
		}
	}
}
