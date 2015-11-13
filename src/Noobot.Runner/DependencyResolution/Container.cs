namespace Noobot.Runner.DependencyResolution
{
    public static class Container
    {
        static Container() { }

        public static StructureMap.Container Instance { get; } = new StructureMap.Container(new DefaultRegistry());
    }
}