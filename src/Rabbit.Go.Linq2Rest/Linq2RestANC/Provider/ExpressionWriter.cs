// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionWriter.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExpressionWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Linq2Rest.Provider.Writers;
#if NETFX_CORE
	//using Linq2Rest.Reactive.Provider;
#endif
#if SILVERLIGHT
	using Linq2Rest.Reactive.Provider;
#endif

	internal class ExpressionWriter : IExpressionWriter
	{
		private static readonly ExpressionType[] CompositeExpressionTypes = { ExpressionType.Or, ExpressionType.OrElse, ExpressionType.And, ExpressionType.AndAlso };
		private readonly ParameterValueWriter _valueWriter;
		private readonly IMethodCallWriter[] _methodCallWriters;

		private readonly IMemberNameResolver _memberNameResolver;

		public ExpressionWriter(IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters)
		{
			CustomContract.Requires<ArgumentNullException>(memberNameResolver != null);

			_valueWriter = new ParameterValueWriter(valueWriters ?? new IntValueWriter[0]);
			_memberNameResolver = memberNameResolver;
			_methodCallWriters = new IMethodCallWriter[]
								 {
									 new EqualsMethodWriter(),
									 new StringReplaceMethodWriter(),
									 new StringTrimMethodWriter(),
									 new StringToLowerMethodWriter(),
									 new StringToUpperMethodWriter(),
									 new StringSubstringMethodWriter(),
									 new StringContainsMethodWriter(),
									 new StringIndexOfMethodWriter(),
									 new StringEndsWithMethodWriter(),
									 new StringStartsWithMethodWriter(),
									 new MathRoundMethodWriter(),
									 new MathFloorMethodWriter(),
									 new MathCeilingMethodWriter(),
									 new EmptyAnyMethodWriter(),
									 new AnyAllMethodWriter(),
                                     new InMethodWriter(),
                                     new DefaultMethodWriter(_valueWriter)
								 };
		}

		public string Write(Expression expression, Type sourceType)
		{
			return expression == null ? null : Write(expression, expression.Type, GetRootParameterName(expression), sourceType);
		}

		private static Type GetUnconvertedType(Expression expression)
		{
			CustomContract.Requires(expression != null);

			switch (expression.NodeType)
			{
				case ExpressionType.Convert:
					var unaryExpression = expression as UnaryExpression;

					CustomContract.Assume(unaryExpression != null, "Matches node type.");

					return unaryExpression.Operand.Type;
				default:
					return expression.Type;
			}
		}

		private static string GetMemberCall(MemberExpression memberExpression)
		{
			CustomContract.Requires(memberExpression != null);
			CustomContract.Ensures(CustomContract.Result<string>() != null);

			var declaringType = memberExpression.Member.DeclaringType;
			var name = memberExpression.Member.Name;

			if (declaringType == typeof(string) && string.Equals(name, "Length"))
			{
				return name.ToLowerInvariant();
			}

			if (declaringType == typeof(DateTime))
			{
				switch (name)
				{
					case "Hour":
					case "Minute":
					case "Second":
					case "Day":
					case "Month":
					case "Year":
						return name.ToLowerInvariant();
				}
			}

			return string.Empty;
		}

		private static Expression CollapseCapturedOuterVariables(MemberExpression input)
		{
			if (input == null || input.NodeType != ExpressionType.MemberAccess)
			{
				return input;
			}

			switch (input.Expression.NodeType)
			{
				case ExpressionType.New:
				case ExpressionType.MemberAccess:
					var value = GetValue(input);
					return Expression.Constant(value);
				case ExpressionType.Constant:
					var obj = ((ConstantExpression)input.Expression).Value;
					if (obj == null)
					{
						return input;
					}

					var fieldInfo = input.Member as FieldInfo;
					if (fieldInfo != null)
					{
						var result = fieldInfo.GetValue(obj);
						return result is Expression ? (Expression)result : Expression.Constant(result);
					}

					var propertyInfo = input.Member as PropertyInfo;
					if (propertyInfo != null)
					{
						var result = propertyInfo.GetValue(obj, null);
						return result is Expression ? (Expression)result : Expression.Constant(result);
					}

					break;
				case ExpressionType.TypeAs:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return Expression.Constant(GetValue(input));
			}

			return input;
		}

		private static object GetValue(Expression input)
		{
			CustomContract.Requires(input != null);

			var objectMember = Expression.Convert(input, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

			return getterLambda();
		}

		private static bool IsMemberOfParameter(MemberExpression input)
		{
			if (input == null || input.Expression == null)
			{
				return false;
			}

			var nodeType = input.Expression.NodeType;
			var tempExpression = input.Expression as MemberExpression;
			while (nodeType == ExpressionType.MemberAccess)
			{
				if (tempExpression == null || tempExpression.Expression == null)
				{
					return false;
				}

				nodeType = tempExpression.Expression.NodeType;
				tempExpression = tempExpression.Expression as MemberExpression;
			}

			return nodeType == ExpressionType.Parameter;
		}

		private static string GetOperation(Expression expression)
		{
			CustomContract.Requires(expression != null);

			switch (expression.NodeType)
			{
				case ExpressionType.Add:
					return "add";
				case ExpressionType.AddChecked:
					break;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return "and";
				case ExpressionType.Divide:
					return "div";
				case ExpressionType.Equal:
					return "eq";
				case ExpressionType.GreaterThan:
					return "gt";
				case ExpressionType.GreaterThanOrEqual:
					return "ge";
				case ExpressionType.LessThan:
					return "lt";
				case ExpressionType.LessThanOrEqual:
					return "le";
				case ExpressionType.Modulo:
					return "mod";
				case ExpressionType.Multiply:
					return "mul";
				case ExpressionType.Not:
					return "not";
				case ExpressionType.NotEqual:
					return "ne";
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return "or";
				case ExpressionType.Subtract:
					return "sub";
			}

			return string.Empty;
		}

		private static ParameterExpression GetRootParameterName(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			if (expression is LambdaExpression && ((LambdaExpression)expression).Parameters.Any())
			{
				return ((LambdaExpression)expression).Parameters.First();
			}

			return null;
		}

		private string Write(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			return expression == null ? null : Write(expression, expression.Type, rootParameterName, sourceType);
		}

		private string Write(Expression expression, Type type, ParameterExpression rootParameter, Type sourceType)
		{
			CustomContract.Requires(expression != null);
			CustomContract.Requires(type != null);

			switch (expression.NodeType)
			{
				case ExpressionType.Parameter:
					var parameterExpression = expression as ParameterExpression;

					CustomContract.Assume(parameterExpression != null);

					return parameterExpression.Name;
				case ExpressionType.Constant:
					{
						var value = GetValue(Expression.Convert(expression, type));
						return _valueWriter.Write(value);
					}

				case ExpressionType.Add:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Divide:
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Subtract:
					return WriteBinaryExpression(expression, rootParameter, sourceType);
				case ExpressionType.Negate:
					return WriteNegate(expression, rootParameter, sourceType);
				case ExpressionType.Not:
#if !SILVERLIGHT
				case ExpressionType.IsFalse:
#endif
					return WriteFalse(expression, rootParameter, sourceType);
#if !SILVERLIGHT
				case ExpressionType.IsTrue:
					return WriteTrue(expression, rootParameter, sourceType);
#endif
				case ExpressionType.Convert:
				case ExpressionType.Quote:
					return WriteConversion(expression, rootParameter, sourceType);
				case ExpressionType.MemberAccess:
					return WriteMemberAccess(expression, rootParameter, sourceType);
				case ExpressionType.Call:
					return WriteCall(expression, rootParameter, sourceType);
				case ExpressionType.New:
				case ExpressionType.ArrayIndex:
				case ExpressionType.ArrayLength:
				case ExpressionType.Conditional:
				case ExpressionType.Coalesce:
					var newValue = GetValue(expression);
					return _valueWriter.Write(newValue);
				case ExpressionType.Lambda:
					return WriteLambda(expression, rootParameter, sourceType);
				default:
					throw new InvalidOperationException("Expression is not recognized or supported");
			}
		}

		private string WriteLambda(Expression expression, ParameterExpression rootParameter, Type sourceType)
		{
			var lambdaExpression = expression as LambdaExpression;

			CustomContract.Assume(lambdaExpression != null);

			var body = lambdaExpression.Body;
			return Write(body, rootParameter, sourceType);
		}

		private string WriteFalse(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var unaryExpression = expression as UnaryExpression;

			CustomContract.Assume(unaryExpression != null);

			var operand = unaryExpression.Operand;

			return string.Format("not({0})", Write(operand, rootParameterName, sourceType));
		}

		private string WriteTrue(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var unaryExpression = expression as UnaryExpression;

			CustomContract.Assume(unaryExpression != null);

			var operand = unaryExpression.Operand;

			return Write(operand, rootParameterName, sourceType);
		}

		private string WriteConversion(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var unaryExpression = expression as UnaryExpression;

			CustomContract.Assume(unaryExpression != null);

			var operand = unaryExpression.Operand;
			return Write(operand, rootParameterName, sourceType);
		}

		private string WriteCall(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var methodCallExpression = expression as MethodCallExpression;

			CustomContract.Assume(methodCallExpression != null);

			return GetMethodCall(methodCallExpression, rootParameterName, sourceType);
		}

		private string WriteMemberAccess(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var memberExpression = expression as MemberExpression;

			CustomContract.Assume(memberExpression != null);

			if (memberExpression.Expression == null)
			{
				var memberValue = GetValue(memberExpression);
				return _valueWriter.Write(memberValue);
			}

			var pathPrefixes = new List<MemberInfo>();

			var currentMemberExpression = memberExpression;
			while (true)
			{
				pathPrefixes.Add(currentMemberExpression.Member);

				var currentMember = currentMemberExpression.Expression as MemberExpression;
				if (currentMember == null)
				{
					break;
				}

				currentMemberExpression = currentMember;
			}

			pathPrefixes.Reverse();
			var prefix = string.Join("/", pathPrefixes.Select(x => _memberNameResolver.GetNameFromAlias(x, sourceType)).Select(x => x.Item2));
			if (rootParameterName != null
				&& currentMemberExpression.Expression is ParameterExpression
				&& ((ParameterExpression)currentMemberExpression.Expression).Name != rootParameterName.Name)
			{
				prefix = string.Format("{0}/{1}", ((ParameterExpression)currentMemberExpression.Expression).Name, prefix);
			}

			if (!IsMemberOfParameter(memberExpression))
			{
				var collapsedExpression = CollapseCapturedOuterVariables(memberExpression);
				if (!(collapsedExpression is MemberExpression))
				{
					CustomContract.Assume(collapsedExpression != null);

					return Write(collapsedExpression, rootParameterName, sourceType);
				}

				memberExpression = (MemberExpression)collapsedExpression;
			}

			var memberCall = GetMemberCall(memberExpression);

			var innerExpression = memberExpression.Expression;

			CustomContract.Assume(innerExpression != null);

			return string.IsNullOrWhiteSpace(memberCall)
					   ? prefix
					   : string.Format("{0}({1})", memberCall, Write(innerExpression, rootParameterName, sourceType));
		}

		private string WriteNegate(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var unaryExpression = expression as UnaryExpression;

			CustomContract.Assume(unaryExpression != null);

			var operand = unaryExpression.Operand;

			return string.Format("-{0}", Write(operand, rootParameterName, sourceType));
		}

		private string WriteBinaryExpression(Expression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			var binaryExpression = expression as BinaryExpression;

			CustomContract.Assume(binaryExpression != null);

			var operation = GetOperation(binaryExpression);

			if (binaryExpression.Left.NodeType == ExpressionType.Call)
			{
				var compareResult = ResolveCompareToOperation(
					rootParameterName,
					(MethodCallExpression)binaryExpression.Left,
					operation,
					binaryExpression.Right as ConstantExpression,
					sourceType);
				if (compareResult != null)
				{
					return compareResult;
				}
			}

			if (binaryExpression.Right.NodeType == ExpressionType.Call)
			{
				var compareResult = ResolveCompareToOperation(
					rootParameterName,
					(MethodCallExpression)binaryExpression.Right,
					operation,
					binaryExpression.Left as ConstantExpression,
					sourceType);
				if (compareResult != null)
				{
					return compareResult;
				}
			}

			var isLeftComposite = CompositeExpressionTypes.Any(x => x == binaryExpression.Left.NodeType);
			var isRightComposite = CompositeExpressionTypes.Any(x => x == binaryExpression.Right.NodeType);

			var leftType = GetUnconvertedType(binaryExpression.Left);
			var leftString = Write(binaryExpression.Left, rootParameterName, sourceType);
			var rightString = Write(binaryExpression.Right, leftType, rootParameterName, sourceType);

			return string.Format(
				"{0} {1} {2}",
				string.Format(isLeftComposite ? "({0})" : "{0}", leftString),
				operation,
				string.Format(isRightComposite ? "({0})" : "{0}", rightString));
		}

		private string ResolveCompareToOperation(
			ParameterExpression rootParameterName,
			MethodCallExpression methodCallExpression,
			string operation,
			ConstantExpression comparisonExpression,
			Type sourceType)
		{
			if (methodCallExpression != null
				&& methodCallExpression.Method.Name == "CompareTo"
				&& methodCallExpression.Method.ReturnType == typeof(int)
				&& comparisonExpression != null
				&& Equals(comparisonExpression.Value, 0))
			{
				return string.Format(
					"{0} {1} {2}",
					Write(methodCallExpression.Object, rootParameterName, sourceType),
					operation,
					Write(methodCallExpression.Arguments[0], rootParameterName, sourceType));
			}

			return null;
		}

		private string GetMethodCall(MethodCallExpression expression, ParameterExpression rootParameterName, Type sourceType)
		{
			CustomContract.Requires(expression != null);

			var methodCallWriter = _methodCallWriters.FirstOrDefault(w => w.CanHandle(expression));
			if (methodCallWriter == null)
			{
				throw new NotSupportedException(expression + " is not supported");
			}

			return methodCallWriter.Handle(expression, e => Write(e, rootParameterName, sourceType));
		}
	}
}