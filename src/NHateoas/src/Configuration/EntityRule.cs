using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class EntityRule
    {
        public enum EmbeddingRule
        {
            Embedded,
            Linked
        }

        private readonly MethodCallExpression _actionSelector;
        private readonly Delegate _entityGetter;

        public EntityRule(Expression entitySelector, MethodCallExpression actionSelector, EmbeddingRule rule, string[] rel)
        {
            Rel = rel;
            EntityEmbeddingRule = rule;
            _actionSelector = actionSelector;
            _entityGetter = CreateDelegateFromExpression(entitySelector);
        }

        static Delegate CreateDelegateFromExpression(Expression expr)
        {
            var expression = expr as LambdaExpression;
            
            if (expression == null)
                throw new Exception("Unsupported expression for Embedded Entity selector");

            return expression.Compile();     
        }

        public Type ControllerType
        {
            get { return _actionSelector.Method.DeclaringType; }
        }
        
        public MethodInfo ControllerAction
        {
            get { return _actionSelector.Method; }
        }

        public object GetReferencedObjectInstance(object sourceObject)
        {
            return _entityGetter.DynamicInvoke(sourceObject);
        }

        public EmbeddingRule EntityEmbeddingRule { get; internal set; }

        public string[] Rel { get; internal set; }
    }
}
