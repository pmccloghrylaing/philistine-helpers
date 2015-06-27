using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philistine.Monads
{
	public interface ITry : IEquatable<ITry>
	{
		bool IsSuccess { get; }

		object Value { get; }

		Exception Exception { get; }
	}

	public interface ITry<out T> : ITry
	{
		bool IsSuccess { get; }

		T Value { get; }

		Exception Exception { get; }
	}


	public class Success<T> : ITry<T>, ITry
	{
		public Success(T value)
		{
			this.Value = value;
		}

		object ITry.Value { get { return Value; } }
		public T Value { get; private set; }

		public Exception Exception
		{
			get { throw new NotSupportedException(); }
		}

		public bool IsSuccess { get { return true; } }

		public override bool Equals(object obj)
		{
			return Equals(obj as ITry);
		}

		public bool Equals(ITry other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			if (ReferenceEquals(other, this))
			{
				return true;
			}
			if (!other.IsSuccess)
			{
				return false;
			}
			return Equals(other.Value, Value);
		}

		public override int GetHashCode()
		{
			return Value != null ? Value.GetHashCode() : 0;
		}
	}

	public class Failure<T> : ITry<T>, ITry
	{
		public Failure(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.Exception = exception;
		}

		object ITry.Value { get { return Value; } }
		public T Value
		{
			get { throw new NotSupportedException(); }
		}

		public Exception Exception { get; private set; }

		public bool IsSuccess { get { return false; } }

		public override bool Equals(object obj)
		{
			return Equals(obj as ITry);
		}

		public bool Equals(ITry other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			if (ReferenceEquals(other, this))
			{
				return true;
			}
			if (other.IsSuccess)
			{
				return false;
			}
			return Equals(other.Exception, Exception);
		}

		public override int GetHashCode()
		{
			return Exception != null ? Exception.GetHashCode() : 0;
		}
	}
}
