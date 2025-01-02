﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MineDirt;
using System.Collections.Generic;
public class Chunk
{
    public static int Height { get; private set; } = 64; // Max chunk size
    public Vector3 Position { get; private set; }

    // List to store all subchunks
    public List<Subchunk> Subchunks { get; private set; }

    public Chunk(Vector3 position)
    {
        Position = position;
        Subchunks = [];

        // Generate subchunks
        GenerateSubchunks();
    }

    private void GenerateSubchunks()
    {
        int subchunkCount = Height / Subchunk.Size;

        for (int y = 0; y < subchunkCount; y++)
        {
            Vector3 subchunkPosition = new(Position.X, y * Subchunk.Size, Position.Z);

            // Create and add the subchunk
            Subchunks.Add(new Subchunk(subchunkPosition));
        }
    }

    public void Draw(BasicEffect effect)
    {
        for (int i = 0; i < Subchunks.Count; i++)
            Subchunks[i].Draw(effect);
    }
}
