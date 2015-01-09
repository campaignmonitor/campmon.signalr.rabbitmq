using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Publish
{
    public class UnwrapMethodCall
    {
        private readonly MethodCallExpression expression;
        private readonly Lazy<object[]> arguments;

        public UnwrapMethodCall(Expression expression)
            : this(expression as MethodCallExpression)
        {

        }

        public UnwrapMethodCall(MethodCallExpression expression)
        {
            if (expression == null) throw new ArgumentException("Must be a method call expression", "expression");
            this.expression = expression;
            arguments = new Lazy<object[]>(() => expression
                .Arguments
                .Select(p => Expression.Lambda(p)
                    .Compile()
                    .DynamicInvoke())
                .ToArray());
        }

        public string MethodName
        {
            get { return expression.Method.Name; }
        }

        public object[] Arguments
        {
            get
            {
                return arguments.Value;
            }
        }
    }
}
