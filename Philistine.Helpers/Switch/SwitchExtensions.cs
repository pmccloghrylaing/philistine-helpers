using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Philistine
{
	public static class SwitchExtensions
	{
		public static ISwitch<TControl> Switch<TControl>(this TControl control)
		{
			return new Unresolved<TControl, object>(control);
		}

		public static ISwitch<TControl, TResult> Case<TControl, T, TResult>(this ISwitch<TControl> source, Predicate<T> predicate, Func<T, TResult> selector)
		{
			return source.Case(x => x is T && predicate((T)(object)x), x => selector((T)(object)x));
		}

		public static ISwitch<TControl, TResult> Case<TControl, T, TResult>(this ISwitch<TControl> source, Func<T, TResult> selector)
		{
			return source.Case((T x) => true, selector);
		}

		public static ISwitch<TControl, TResult> CaseConstructed<TControl, T, TResult>(this ISwitch<TControl> source, Expression<Func<T>> constructor, Func<T, TResult> selector)
		{
			return CaseConstructedCore<TControl, T, TResult>(source, constructor, selector);
		}

		public static ISwitch<TControl, TResult> CaseConstructed<TControl, T, T1, TResult>(this ISwitch<TControl> source, Expression<Func<T1, T>> constructor, Func<T, T1, TResult> selector)
		{
			return CaseConstructedCore<TControl, T, TResult>(source, constructor, selector);
		}

		internal static ISwitch<TControl, TResult> CaseConstructedCore<TControl, T, TResult>(ISwitch<TControl> source,
			LambdaExpression expression, Delegate selector)
		{
			var constructorParameterTypes = expression.Parameters
				.Select(pe => pe.Type)
				.ToArray();
			var selectorParameterTypes = selector.Method.GetParameters()
				.Select(pi => pi.ParameterType)
				.ToArray();

			if (!typeof(T).IsAssignableFrom(expression.ReturnType))
			{
				throw new ArgumentException("constructor does not return type T: "
					+ typeof(T).Name + " vs " + expression.ReturnType.Name);
			}
			if (!selectorParameterTypes[0].IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("first selector parameter is not of type T: "
					+ typeof(T).Name + " vs " + expression.ReturnType.Name);
			}

			if (constructorParameterTypes.Length < selectorParameterTypes.Length - 1)
			{
				throw new ArgumentException("not enough constructor parameters for selector");
			}

			if (!constructorParameterTypes
				.Zip(selectorParameterTypes.Skip(1), (a, b) => b == null || b.IsAssignableFrom(a))
				.All(x => x))
			{
				throw new ArgumentException("constructor and selector parameters do not match");
			}

			Predicate<T> predicate;
			Func<T, Delegate, TResult> selectWrapper;

			new ConstructorVisitor().GetConditionAndInvoke(expression, out predicate, out selectWrapper);

			return source.Case(predicate, x => selectWrapper(x, selector));
		}
	}
}
