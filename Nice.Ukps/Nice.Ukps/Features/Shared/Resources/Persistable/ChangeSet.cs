using System;
using Action = Nice.Ukps.Resources.Action;

namespace Nice.Ukps.Persistable
{
    public class ChangeSet
    {
        public ChangeSet()
        {
            AggregateRootChanges = new Change[] { };
        }

        public string Id { get; set; }
        public string TechnologyId { get; set; }
        public Action Action { get; set; }
        public int Count { get { return AggregateRootChanges.Length; } }
        public Change[] AggregateRootChanges { get; set; }
        public Change[] AggregateChildChanges { get; set; }
    }

    public class Change
    {
        public string Which { get; set; }
        public object Original { get; set; }
        public object New { get; set; }
        public string Who { get; set; }
        public DateTime When { get; set; }
    }
}