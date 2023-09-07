using Unity.Entities;

namespace TerrainGenerator
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    public partial class OctreeSystemGroup : ComponentSystemGroup
    {
    }

    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    public partial class TerrainGeneratorSystemGroup : ComponentSystemGroup
    {
    }
}
