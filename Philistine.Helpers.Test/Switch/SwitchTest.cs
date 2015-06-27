using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Philistine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Philistine.Monads;

namespace Philistine.Helpers.Test.Switch
{
	[TestClass]
	public class SwitchTest
	{
		[TestMethod]
		public void SwitchInt()
		{
			1.Switch()
				.Case((string x) => false)
				.Case((int x) => true)
				.Default(x => false)
				.Should().BeTrue();

			1.Switch()
				.Case((string x) => false)
				.Case((int x) => x / (x - 1) == 0, x => false)
				.Default(x => true)
				.Should().BeTrue();

			(1 as int?).Switch()
				.Case((int x) => true)
				.Default(x => false)
				.Should().BeTrue();
		}

		[TestMethod]
		public void Constructed()
		{
			Try.Success(1.0).Switch<ITry>()
				.CaseConstructed(() => new Success<double>(1.0),
					(x) => true)
				.Default(x => false)
				.Should().BeTrue();

			Try.Success(1.0).Switch<ITry>()
				.CaseConstructed(() => new Success<double>(1.0),
					(x) => true)
				.Default(x => false)
				.Should().BeTrue();

			Try.Success(1.0).Switch<ITry>()
				.CaseConstructed((double d) => new Success<double>(d),
					(x, d) => d)
				.Default(x => 0)
				.Should().Be(1.0);
		}
	}
}
