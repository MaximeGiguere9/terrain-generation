# Terrain Generation

## Goals

- Play with procedural generation to learn about noise and mesh manipulation.
- Generate, move around, and modify terrain.

## Challenges

- Blocks cannot be individual objects. They must be clustered by chunk and meshes/collision generated by hand.
- Generating chunk meshes is a resource-intensive operation. The profiler helped analyze and optimize memory allocation and cpu usage.
- Water must be transparent and not have collision, which requires creating different meshes for solid/transparent/collision layers.

## Limitations

The following limitations are known but were considered out of scope for this project:
- Structure generation does not work well across chunk boundaries
- Blocks cannot have different sizes (noticeable with cacti).
- Sprint and Place Block are bound to the same input button.
- No save/load
- The player can only place bricks.

## Usage

Configure terrain generation settings with the `Resources/TerrainGeneratorSettings` asset. Launch the `Scenes/VoxelWorld` scene and move around to see what kind of world is generated. Place and break blocks to modify the terrian in realtime.