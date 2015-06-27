using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philistine
{
	public interface ISwitch<out TControl>
	{
		ISwitch<TControl, TResult> Case<TResult>(Predicate<TControl> predicate, Func<TControl, TResult> selector);

		TResult Default<TResult>(Func<TControl, TResult> selector);
	}

	public interface ISwitch<out TControl, TResult> : ISwitch<TControl>
	{
		ISwitch<TControl, TResult> Case(Predicate<TControl> predicate, Func<TControl, TResult> selector);

		TResult Default(Func<TControl, TResult> selector);
	}
}
