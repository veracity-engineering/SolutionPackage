namespace DNVGL.Web.Security.PermissionsPolicies
{

    public class Feature
    {
        public string Name { get; private set; }
        internal Feature(string name)
        {
            Name = name;
        }

        public bool Enabled
        {
            get { return builder.Enabled; }
        }

        private FeatureBuilder builder = new FeatureBuilder();
        public FeatureBuilder Enable()
        {
            builder.Enable();
            return builder;
        }

        public void Disable()
        {
            builder.Disable();
        }

        public override string ToString()
        {
            return $"{Name}={builder.ToString()}";
        }
    }
}

