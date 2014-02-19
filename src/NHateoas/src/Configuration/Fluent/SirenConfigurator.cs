using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration.Fluent
{
    [SecuritySafeCritical]
    public class SirenConfigurator<TModel, TController>
    {
        private readonly HypermediaConfigurator<TModel, TController> _hypermediaConfigurator;
        private readonly HypermediaConfigurationLogic<TModel, TController> _logic;
        
        private const string _selfName = "self";
        private const string _prevName = "previous";
        private const string _nextName = "next";
        private const string _parentName = "parent";

        internal SirenConfigurator(HypermediaConfigurator<TModel, TController> hypermediaConfigurator, HypermediaConfigurationLogic<TModel, TController> logic)
        {
            _hypermediaConfigurator = hypermediaConfigurator;
            _logic = logic;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return _hypermediaConfigurator;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, TModel>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return _hypermediaConfigurator;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Action<TModel, TController>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return _hypermediaConfigurator;
        }

        public SirenConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, TModel>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> Map(Expression<Action<TModel, TController>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Func<TModel, TOtherController, Object>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Action<TModel, TOtherController>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsSelfLink()
        {
            SetActionNameType("AsSelfLink", _selfName, true, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsNextLink()
        {
            SetActionNameType("AsNextLink", _nextName, true, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsPrevLink()
        {
            SetActionNameType("AsPrevLink", _prevName, true, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsParentLink()
        {
            SetActionNameType("AsParentLink", _parentName, true, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsLinkWithName(string name)
        {
            SetActionNameType("AsLinkWithName", name, false, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsActionWithName(string name)
        {
            SetActionNameType("AsActionWithName", name, false, MappingRule.RuleType.ActionRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsLink()
        {
            SetActionNameType("AsLink", string.Empty, false, MappingRule.RuleType.LinkRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> AsAction()
        {
            SetActionNameType("AsAction", string.Empty, false, MappingRule.RuleType.ActionRule);
            return this;
        }

        public SirenConfigurator<TModel, TController> WithContentType(string contentType)
        {
            if (_logic.ActionConfigurationMappingRule == null)
                throw new Exception("WithContentType should be used after MapXX method");
            
            _logic.ActionConfigurationMappingRule.ContentType = new ContentType(contentType);

            return this;
        }

        public void Configure()
        {
            _hypermediaConfigurator.Configure();
        }

        private void SetActionNameType(string methodName, string rel, bool clear, MappingRule.RuleType type)
        {
            if (_logic.ActionConfigurationMappingRule == null)
                throw new Exception(string.Format("{0} should be used after MapXX method", methodName));

            if (clear)
                _logic.ActionConfigurationMappingRule.Names.Clear();
            if (!string.IsNullOrEmpty(rel))
                _logic.ActionConfigurationMappingRule.Names.Add(rel);

            _logic.ActionConfigurationMappingRule.Type = type;
        }


    }
}
