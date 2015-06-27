using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philistine.Monads
{
	public delegate bool TryFunc<TOut>(out TOut outArg);
	public delegate bool TryFunc<in T1, TOut>(T1 arg1, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, TOut>(T1 arg1, T2 arg2, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, TOut>(T1 arg1, T2 arg2, T3 arg3, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, in T4, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, out TOut outArg);
	public delegate bool TryFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, out TOut outArg);
}
