using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Philistine.Monads;
using System.Text.RegularExpressions;

namespace Philistine.Helpers.Test.Monads
{
	[TestClass]
	public class TryTest
	{
		[TestMethod]
		public void Success()
		{
			var result = Try.Make(() => Return<object>());

			result.IsSuccess.Should().BeTrue();
		}

		[TestMethod]
		public void Failure()
		{
			var result = Try.Make(() => Throw<object>());

			result.IsSuccess.Should().BeFalse();
		}

		static T Throw<T>(Exception ex = null)
		{
			throw ex ?? new Exception();
		}
		static T Return<T>(T value = default(T))
		{
			return value;
		}
	}
}
