namespace PrototypePattern
{
    using System.Collections.Generic;
    public class SandwichMenu
    {
        private readonly Dictionary<string, SandwichPrototype> sandwiches = new Dictionary<string, SandwichPrototype>();

        public SandwichPrototype this[string name]
        {
            get { return sandwiches[name]; }
            set { sandwiches.Add(name,value); }
        }
    }
}
