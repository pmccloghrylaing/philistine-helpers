using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Philistine;

namespace Philistine.Helpers.Test.Switch
{
	[TestClass]
	public class VisitorTest
	{
		class TestClass
		{
			public TestClass(string value)
			{
				this.Value = value;
			}

			public string Value { get; private set; }
			public int OtherValue { get; set; }
		}

		[TestMethod]
		public void GetConditionAndInvoke()
		{
			var expression = Expression((int i) => new TestClass("ABC")
			{
				OtherValue = i
			});

			Predicate<TestClass> predicate;
			Func<TestClass, Delegate, object> selector;

			new ConstructorVisitor().GetConditionAndInvoke(expression, out predicate, out selector);

			var testObj = new TestClass("ABC") { OtherValue = 123 };

			predicate(testObj).Should().BeTrue();

			selector(testObj, getOtherValue).Should().Be(123);
			selector(testObj, getObj).Should().Be(testObj);
		}

		Func<TestClass, int, int> getOtherValue = (TestClass obj, int otherValue) =>
		{
			return otherValue;
		};
		Func<TestClass, int, TestClass> getObj = (TestClass obj, int otherValue) =>
		{
			return obj;
		};

		static Expression<Func<T1, TResult>> Expression<T1, TResult>(Expression<Func<T1, TResult>> expression)
		{
			return expression;
		}
	}
}
