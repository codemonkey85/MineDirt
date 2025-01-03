﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineDirt.Src;
public static class World
{
    public static List<Chunk> Chunks = [];
    public static short RenderDistance { get; private set; } = 8;

    public static void UpdateChunks()
    {
        Vector3 cameraPosition = MineDirtGame.Camera.Position;
        int chunkSize = Subchunk.Size; // Assuming Subchunk.Size is 16
        float renderDistanceSquared = RenderDistance * RenderDistance * chunkSize * chunkSize;  // Square of the radius to avoid sqrt calculation

        // HashSet for efficient chunk presence checks
        HashSet<Vector3> chunksToKeep = new();

        // Loop through chunks within the render distance (in terms of chunk count, not world units)
        for (int x = -RenderDistance; x <= RenderDistance; x++)
        {
            for (int z = -RenderDistance; z <= RenderDistance; z++)
            {
                // Calculate the chunk's world position based on the camera's position and chunk size
                Vector3 chunkPosition = new(
                    (int)(Math.Floor(cameraPosition.X / chunkSize) + x) * chunkSize,
                    0, // Assuming Y is always 0 for simplicity
                    (int)(Math.Floor(cameraPosition.Z / chunkSize) + z) * chunkSize
                );

                Vector3 chunkCenter = chunkPosition + new Vector3(chunkSize / 2, 0, chunkSize / 2);
                float distanceSquared = Vector3.DistanceSquared(
                    new Vector3(cameraPosition.X, 0, cameraPosition.Z ),
                    chunkCenter
                );

                // If the chunk is within the render distance (in squared distance to avoid sqrt)
                if (distanceSquared <= renderDistanceSquared)
                {
                    chunksToKeep.Add(chunkPosition);

                    // Add new chunk if it doesn't already exist
                    if (!Chunks.Any(chunk => chunk.Position == chunkPosition))
                    {
                        Chunks.Add(new Chunk(chunkPosition));
                    }
                }
            }
        }

        // Remove chunks outside the render distance
        Chunks.RemoveAll(chunk =>
        {
            if (!chunksToKeep.Contains(chunk.Position))
            {
                return true;
            }
            return false;
        });
    }


    public static void DrawChunks(BasicEffect effect)
    {
        // Get the camera's frustum
        BoundingFrustum frustum = new(effect.View * effect.Projection);

        for (int i = 0; i < Chunks.Count; i++)
        {
            // Create a bounding box for the subchunk
            BoundingBox subchunkBoundingBox = new(
                Chunks[i].Position - new Vector3(Subchunk.Size, Chunk.Height, Subchunk.Size),
                Chunks[i].Position + new Vector3(Subchunk.Size, Chunk.Height, Subchunk.Size)
            );

            // Check if the bounding box is inside the frustum
            if (frustum.Contains(subchunkBoundingBox) != ContainmentType.Disjoint)
            {
                // Only draw subchunks within the frustum
                Chunks[i].Draw(effect);
            }
        }
    }
}
