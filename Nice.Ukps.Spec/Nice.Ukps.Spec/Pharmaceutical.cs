using System;
using System.Linq;
using Machine.Specifications;
using Nice.Ukps.Features.Pharmaceutical.Services;
using Nice.Ukps.Resources;
using Nice.Ukps.Spec.contexts;
using Pharmaceutical = Nice.Ukps.Features.Pharmaceutical.Services.Pharmaceutical;

namespace Nice.Ukps.Spec
{
    public class when_saving_a_new_horizonScanner : service_with_raven_persistence<Pharmaceutical>
    {
        static Resources.Pharmaceutical input;

        Establish context = () => input = new Resources.Pharmaceutical
                                              {
                                                  Title = "Title"
                                              };

        Because of = () => input = Service.PutResource(input);
        It has_an_id = () => input.Id.ShouldNotBeNull();
        It has_correct_id = () => input.Id.ShouldEqual("technologies/1");
        It has_the_title = () => input.Title.ShouldEqual("Title");
    }

    public class when_generating_audit_for_new_technology_with_no_collection_changes
    {
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
            {
                input = new Resources.Pharmaceutical
                            {
                                Title = "Title",
                                CommercialInConfidence = "bob and bruce",
                                Count = 1,
                                Decide = true,
                                Description = "billy",
                                When = new DateTime(2011, 1, 1)
                            };
            };

        Because of = () => result = input.GetChangeSet(new Resources.Pharmaceutical(), "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_a_create = () => result.Action.ShouldEqual(Resources.Action.Create);
        It knows_six_values_changed = () => result.Count.ShouldEqual(6);
        It has_a_well_formed_value_for_title = () =>
            {
                result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual(null);
                result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("Title");
                result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
            };
    }

    public class when_generating_audit_for_new_technology_with_collection_changes
    {
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
        {
            input = new Resources.Pharmaceutical
            {
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Value1 = "one" } },
                When = new DateTime(2011, 1, 1)
            };
        };

        Because of = () => result = input.GetChangeSet(new Resources.Pharmaceutical(), "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_a_create = () => result.Action.ShouldEqual(Resources.Action.Create);
        It knows_child_values_changed = () => result.Count.ShouldEqual(7);
        It has_a_well_formed_value_for_title = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual(null);
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
        };
        It has_a_well_formed_value_for_children = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Original.ShouldEqual("Did have 0");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().New.ShouldEqual("Now has 1");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Who.ShouldEqual("test account");
        };
    }

    public class when_generating_audit_for_exisiting_technology_with_one_change
    {
        static Resources.Pharmaceutical original;
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
        {
            original = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                When = new DateTime(2011, 1, 1)
            };
            input = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "New Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                When = new DateTime(2011, 1, 1)
            };
        };

        Because of = () => result = input.GetChangeSet(original, "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_an_edit = () => result.Action.ShouldEqual(Resources.Action.Edit);
        It knows_one_value_changed = () => result.Count.ShouldEqual(1);
        It has_a_well_formed_value_for_title = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual("Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("New Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
        };
    }

    public class when_generating_audit_for_exisiting_technology_with_one_more_collection_added
    {
        static Resources.Pharmaceutical original;
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
        {
            original = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Id = "clinicaltrials/1", Value1 = "one" } },
                When = new DateTime(2011, 1, 1)
            };
            input = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "New Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Id = "clinicaltrials/1", Value1 = "one" }, new ClinicalTrial { Id = "clinicaltrials/2", Value1 = "two" } },
                When = new DateTime(2011, 1, 1)
            };
        };

        Because of = () => result = input.GetChangeSet(original, "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_an_edit = () => result.Action.ShouldEqual(Resources.Action.Edit);
        It knows_one_value_changed = () => result.Count.ShouldEqual(2);
        It has_a_well_formed_value_for_title = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual("Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("New Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
        };
        It has_a_well_formed_value_for_children = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Original.ShouldEqual("Did have 1");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().New.ShouldEqual("Now has 2");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Who.ShouldEqual("test account");
        };
    }

    public class when_removing_all_children
    {
        static Resources.Pharmaceutical original;
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
        {
            original = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Id = "clinicaltrials/1", Value1 = "one" }, new ClinicalTrial { Id = "clinicaltrials/2", Value1 = "two" } },
                When = new DateTime(2011, 1, 1)
            };
            input = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "New Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                When = new DateTime(2011, 1, 1)
            };
        };

        Because of = () => result = input.GetChangeSet(original, "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_an_edit = () => result.Action.ShouldEqual(Resources.Action.Edit);
        It knows_one_value_changed = () => result.Count.ShouldEqual(2);
        It has_a_well_formed_value_for_title = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual("Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("New Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
        };
        It has_a_well_formed_value_for_children = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Original.ShouldEqual("Did have 2");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().New.ShouldEqual("Now has 0");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Who.ShouldEqual("test account");
        };
    }

    public class when_removing_one_child_and_adding_a_different_child
    {
        static Resources.Pharmaceutical original;
        static Resources.Pharmaceutical input;
        static Persistable.ChangeSet result;
        Establish context = () =>
        {
            original = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Id = "clinicaltrials/1", Value1 = "one" } },
                When = new DateTime(2011, 1, 1)
            };
            input = new Resources.Pharmaceutical
            {
                Id = "technologies/1",
                Title = "Title",
                CommercialInConfidence = "bob and bruce",
                Count = 1,
                Decide = true,
                Description = "billy",
                Children = new[] { new ClinicalTrial { Id = "clinicaltrials/2", Value1 = "two" } },
                When = new DateTime(2011, 1, 1)
            };
        };

        Because of = () => result = input.GetChangeSet(original, "test account");

        It returns_a_changeset = () => result.ShouldNotBeNull();
        It knows_this_is_an_edit = () => result.Action.ShouldEqual(Resources.Action.Edit);
        It knows_one_value_changed = () => result.Count.ShouldEqual(1);
        It has_a_well_formed_value_for_title = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Original.ShouldEqual("Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().New.ShouldEqual("New Title");
            result.AggregateRootChanges.Where(x => x.Which == "Title").FirstOrDefault().Who.ShouldEqual("test account");
        };
        It has_a_well_formed_value_for_children = () =>
        {
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Original.ShouldEqual("Did have 1");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().New.ShouldEqual("Now has 1");
            result.AggregateRootChanges.Where(x => x.Which == "Children").FirstOrDefault().Who.ShouldEqual("test account");
        }; 
    }
}