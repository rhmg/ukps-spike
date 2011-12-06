using System;

namespace Nice.Ukps.Persistable
{
    public class Technology
    {
        public Drug Parent { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        public bool Decide { get; set; }

        public string[] Children { get; set; }

        public DateTime When { get; set; }

        public string CommercialInConfidence { get; set; }
    }

    public class Drug
    {
        public string Id { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
    }
}