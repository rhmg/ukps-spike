using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nice.Ukps.Persistable;

namespace Nice.Ukps.Resources
{
    public class ClinicalTrial
    {
        public string Id { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }
    }

    public class Pharmaceutical
    {
        public Pharmaceutical()
        {
            Children = new ClinicalTrial[] { };
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        public bool Decide { get; set; }

        public ClinicalTrial[] Children { get; set; }

        public DateTime When { get; set; }

        public string CommercialInConfidence { get; set; }

        public ChangeSet GetChangeSet(Pharmaceutical previous, string user)
        {
            var aggregateRootChanges = new List<Change>();
            var hs = GetType();
            var properties = hs.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance |
                                    BindingFlags.GetProperty).ToList().Where(x => x.Name != "Id");
            var action = string.IsNullOrEmpty(this.Id) ? Action.Create : Action.Edit;
            foreach (var property in properties)
            {
                var oldVal = action == Action.Create ? null : hs.GetProperty(property.Name).GetValue(previous, null);
                var newVal = hs.GetProperty(property.Name).GetValue(this, null);
                if (newVal == null && oldVal == null)
                    continue;
                if (newVal == null && oldVal != null ||
                    !Equality(property, previous, oldVal, newVal))
                {
                    if (property.PropertyType.IsArray)
                    {
                        oldVal = string.Format("Did have {0}", action == Action.Create ? "0" : ((Array)hs.GetProperty(property.Name).GetValue(previous, null)).Length.ToString());
                        newVal = string.Format("Now has {0}", ((Array)hs.GetProperty(property.Name).GetValue(this, null)).Length);
                    }
                    aggregateRootChanges.Add(new Change
                                                 {
                                                     Which = property.Name,
                                                     Original = oldVal,
                                                     New = newVal,
                                                     Who = user,
                                                     When = DateTime.Now
                                                 });
                }
            }
            var children = hs.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance |
                                    BindingFlags.GetProperty).ToList().Where(x => x.PropertyType.IsArray);

            return new ChangeSet { TechnologyId = Id, Action = action, AggregateRootChanges = aggregateRootChanges.ToArray() };
        }

        bool Equality(PropertyInfo p1, Pharmaceutical previous, object oldVal, object newVal)
        {
            if (p1.PropertyType.IsArray)
            {
                var oldArray = GetType().GetProperty(p1.Name).GetValue(previous, null) as Array;
                var newArray = GetType().GetProperty(p1.Name).GetValue(this, null) as Array;
                if (oldArray.Length == 0 && newArray.Length == 0)
                    return true;
                if (oldArray.Length == newArray.Length)
                {

                }
                return false;
            }
            return newVal.Equals(oldVal);
        }
    }
}