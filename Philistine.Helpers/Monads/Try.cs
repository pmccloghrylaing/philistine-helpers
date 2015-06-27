using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philistine.Monads
{
	public static class Try
	{
		public static ITry<T> Make<T>(Func<T> func)
		{
			try
			{
				return new Success<T>(func());
			}
			catch (Exception ex)
			{
				return new Failure<T>(ex);
			}
		}

		public static ITry<T> Success<T>(T value)
		{
			return new Success<T>(value);
		}

		public static ITry<T> Failure<T>(Exception exception)
		{
			return new Failure<T>(exception);
		}

		public static ITry<Exception> Failed<T>(this ITry<T> source)
		{
			return Try.Make(() => source.Exception);
		}

		public static ITry<TResult> Bind<TSource, TResult>(this ITry<TSource> source,
			Func<TSource, ITry<TResult>> selector)
		{
			if (source.IsSuccess)
			{
				try
				{
					return selector(source.Value);
				}
				catch (Exception ex)
				{
					return new Failure<TResult>(ex);
				}
			}
			else
			{
				return new Failure<TResult>(source.Exception);
			}
		}

		public static ITry<TResult> SelectMany<TSource, TIntermediate, TResult>(this ITry<TSource> source,
			Func<TSource, ITry<TIntermediate>> trySelector, Func<TSource, TIntermediate, TResult> resultSelector)
		{
			return source.Bind(s => trySelector(s).Bind(i => new Success<TResult>(resultSelector(s, i))));
		}
		public static ITry<TResult> SelectMany<TSource, TResult>(this ITry<TSource> source,
		   Func<TSource, ITry<TResult>> trySelector)
		{
			return source.SelectMany(trySelector, (s, i) => i);
		}

		public static ITry<TResult> Select<TSource, TResult>(this ITry<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Bind(s => new Success<TResult>(selector(s)));
		}

		public static ITry<TSource> Where<TSource>(this ITry<TSource> source, Predicate<TSource> predicate)
		{
			return source.Bind(s =>
			{
				if (!predicate(s))
				{
					throw new Exception("predicate failed");
				}
				return source;
			});
		}


		public static ITry<TResult> Recover<TSource, TResult>(this ITry<TSource> source, Func<Exception, TResult> recovery)
			where TSource : TResult
		{
			return source.RecoverWith(ex => Try.Make(() => recovery(ex)));
		}

		public static ITry<TResult> RecoverWith<TSource, TResult>(this ITry<TSource> source, Func<Exception, ITry<TResult>> recovery)
			where TSource : TResult
		{
			if (source.IsSuccess)
			{
				return new Success<TResult>(source.Value);
			}
			else
			{
				return recovery(source.Exception);
			}
		}

		public static TResult GetValueOrDefault<TSource, TResult>(this ITry<TSource> source)
			where TSource : TResult
		{
			return source.GetValueOrDefault(() => default(TResult));
		}
		public static TResult GetValueOrDefault<TSource, TResult>(this ITry<TSource> source, TResult defaultValue)
			where TSource : TResult
		{
			return source.GetValueOrDefault(() => defaultValue);
		}
		public static TResult GetValueOrDefault<TSource, TResult>(this ITry<TSource> source, Func<TResult> getDefault)
			where TSource : TResult
		{
			return source.IsSuccess ? source.Value : getDefault();
		}

		public static ITry<TResult> OrElse<TSource, TResult>(this ITry<TSource> source, ITry<TResult> defaultValue)
			where TSource : TResult
		{
			return source.OrElse(() => defaultValue);
		}
		public static ITry<TResult> OrElse<TSource, TResult>(this ITry<TSource> source, Func<ITry<TResult>> getDefault)
			where TSource : TResult
		{
			return source.IsSuccess ? new Success<TResult>(source.Value) : getDefault();
		}
	}
}
