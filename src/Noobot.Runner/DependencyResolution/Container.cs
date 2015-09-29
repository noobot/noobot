namespace Noobot.Runner.DependencyResolution
{
    public static class Container
    {
        private static readonly StructureMap.Container _instance = new StructureMap.Container(new DefaultRegistry());
        static Container() { }

        public static StructureMap.Container Instance { get { return _instance; } }
    }
}