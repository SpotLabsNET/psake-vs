namespace PSake.TaskRunner.Completion
{
    using System.Collections.Generic;

    using Microsoft.CSS.Core.Parser;
    using Microsoft.JSON.Core.Parser;

    internal class JSONItemCollector<T> : IJSONSimpleTreeVisitor where T : JSONParseItem
    {
        public IList<T> Items { get; private set; }
        private bool _includeChildren;

        public JSONItemCollector() : this(false) { }

        public JSONItemCollector(bool includeChildren)
        {
            this._includeChildren = includeChildren;
            this.Items = new List<T>();
        }

        public VisitItemResult Visit(JSONParseItem parseItem)
        {
            var item = parseItem as T;

            if (item != null)
            {
                this.Items.Add(item);
                return (this._includeChildren) ? VisitItemResult.Continue : VisitItemResult.SkipChildren;
            }

            return VisitItemResult.Continue;
        }
    }
}
