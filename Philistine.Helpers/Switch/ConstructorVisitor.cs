using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Philistine
{
	class ConstructorVisitor : ExpressionVisitor
	{
		ParameterExpression itemParameter;
		ParameterExpression delegateParameter;
		List<BinaryExpression> predicateConditions;
		Dictionary<ParameterExpression, MemberExpression> propertyAccessors;

		public void GetConditionAndInvoke<T, TResult>(LambdaExpression expression, out Predicate<T> predicate, out Func<T, Delegate, TResult> selector)
		{
			this.itemParameter = Expression.Parameter(typeof(T));
			this.delegateParameter = Expression.Parameter(typeof(Delegate));
			this.predicateConditions = new List<BinaryExpression>();
			this.propertyAccessors = new Dictionary<ParameterExpression, MemberExpression>();

			Visit(expression);

			predicate = Expression.Lambda<Predicate<T>>(
				predicateConditions.Any()
					? predicateConditions.Aggregate(default(Expression),
						(acc, pc) => acc != null
							? Expression.MakeBinary(ExpressionType.AndAlso, acc, pc)
							: pc)
					: Expression.Constant(true),
				itemParameter).Compile();

			selector = Expression.Lambda<Func<T, Delegate, TResult>>(
				Expression.Convert(
					Expression.Call(delegateParameter, "DynamicInvoke", new Type[0],
						Expression.NewArrayInit(typeof(object),
							new Expression[]
							{
								itemParameter
							}
							.Concat(expression.Parameters
								.Select(p => propertyAccessors.ContainsKey(p)
									? (Expression)propertyAccessors[p]
									: Expression.Default(p.Type)))
							.Select(ex => ex.Type == typeof(object) ? ex : Expression.Convert(ex, typeof(object)))
							.ToArray())),
					typeof(TResult)),
				itemParameter,
				delegateParameter).Compile();
		}

		MemberExpression GetPropertyFromParameterInfo(ParameterInfo parameter)
		{
			return Expression.Property(itemParameter,
				itemParameter.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
					.OrderByDescending(pi => pi.DeclaringType == itemParameter.Type)
					.First(pi => pi.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase)
					&& pi.PropertyType.IsAssignableFrom(parameter.ParameterType)));
		}

		protected override Expression VisitNew(NewExpression node)
		{
			var parametersAndMembers = node.Constructor.GetParameters().Cast<object>()
				.Concat((IEnumerable<MemberInfo>)node.Members ?? new MemberInfo[0]).ToArray();

			for (int i = 0; i < node.Arguments.Count; i++)
			{
				var argument = node.Arguments[i];
				var parameter = parametersAndMembers[i] as ParameterInfo;

				MemberExpression propertyExpression;
				if (parameter != null)
				{
					propertyExpression = GetPropertyFromParameterInfo(parameter);
				}
				else
				{
					var member = (MemberInfo)parametersAndMembers[i];
					propertyExpression = Expression.Property(itemParameter, (PropertyInfo)member);
				}

				var parameterExpression = argument as ParameterExpression;
				if (parameterExpression != null)
				{
					propertyAccessors.Add(parameterExpression, propertyExpression);
				}
				else
				{
					predicateConditions.Add(Expression.MakeBinary(ExpressionType.Equal, propertyExpression, argument));
				}
			}
			
			return base.VisitNew(node);
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			var propertyExpression = Expression.Property(itemParameter, (PropertyInfo)node.Member);

			var parameterExpression = node.Expression as ParameterExpression;
			if (parameterExpression != null)
			{
				propertyAccessors.Add(parameterExpression, propertyExpression);
			}
			else
			{
				predicateConditions.Add(Expression.MakeBinary(ExpressionType.Equal, propertyExpression, node.Expression));
			}
			return base.VisitMemberAssignment(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var parameters = node.Method.GetParameters();

			for (int i = 0; i < node.Arguments.Count; i++)
			{
				var argument = node.Arguments[i];

				var propertyExpression = GetPropertyFromParameterInfo(parameters[i]);

				var parameterExpression = argument as ParameterExpression;
				if (parameterExpression != null)
				{
					propertyAccessors.Add(parameterExpression, propertyExpression);
				}
				else
				{
					predicateConditions.Add(Expression.MakeBinary(ExpressionType.Equal, propertyExpression, argument));
				}
			}
			
			return base.VisitMethodCall(node);
		}
	}
}
