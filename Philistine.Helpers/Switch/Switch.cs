using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Philistine
{
	public abstract class Switch<TControl, TResult> : ISwitch<TControl>, ISwitch<TControl, TResult>
	{
		internal Switch()
		{ }

		protected abstract TControl Control { get; }
		public abstract TResult Value { get; }
		public abstract bool HasValue { get; }

		public abstract ISwitch<TControl, TResult> Case(Predicate<TControl> predicate, Func<TControl, TResult> selector);
		public abstract TResult Default(Func<TControl, TResult> selector);

		ISwitch<TControl, TNewResult> ISwitch<TControl>.Case<TNewResult>(Predicate<TControl> predicate, Func<TControl, TNewResult> selector)
		{
			if (HasValue)
			{
				throw new InvalidOperationException();
			}
			return new Unresolved<TControl, TNewResult>(Control).Case(predicate, selector);
		}
		TNewResult ISwitch<TControl>.Default<TNewResult>(Func<TControl, TNewResult> selector)
		{
			if (HasValue)
			{
				throw new InvalidOperationException();
			}
			return new Unresolved<TControl, TNewResult>(Control).Default(selector);
		}
	}

	public class Resolved<TControl, TResult> : Switch<TControl, TResult>
	{
		readonly TResult value;

		public Resolved(TResult value)
		{
			this.value = value;
		}

		protected override TControl Control { get { throw new NotSupportedException(); } }
		public override TResult Value { get { return value; } }
		public override bool HasValue { get { return true; } }

		public override ISwitch<TControl, TResult> Case(Predicate<TControl> predicate, Func<TControl, TResult> action)
		{
			return this;
		}

		public override TResult Default(Func<TControl, TResult> action)
		{
			return Value;
		}
	}

	public class Unresolved<TControl, TResult> : Switch<TControl, TResult>
	{
		readonly TControl control;

		public Unresolved(TControl control)
		{
			this.control = control;
		}

		protected override TControl Control { get {return control;} }
		public override TResult Value { get { throw new NotSupportedException(); } }
		public override bool HasValue { get { return false; } }

		public override ISwitch<TControl, TResult> Case(Predicate<TControl> predicate, Func<TControl, TResult> selector)
		{
			try
			{
				if (!predicate(Control))
				{
					return this;
				}
			}
			catch
			{
				return this;
			}
			return new Resolved<TControl, TResult>(selector(Control));
		}

		public override TResult Default(Func<TControl, TResult> action)
		{
			return action(Control);
		}
	}
}
