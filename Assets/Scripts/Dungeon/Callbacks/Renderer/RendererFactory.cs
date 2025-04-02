
namespace Dungeon.Renderer {

    public static class RendererFactory {
        public static DungeonCallback CreateRenderer(DungeonRendererWrapper renderer) {
            switch (renderer.type) {
                case DungeonRendererWrapper.RenderType.Walls:
                    return new DungeonWallRenderer(renderer.tilemap, renderer.config);

                case DungeonRendererWrapper.RenderType.Normal:
                default:
                    return new DungeonRenderer(renderer.tilemap, renderer.config);
            }
        }
    }
}