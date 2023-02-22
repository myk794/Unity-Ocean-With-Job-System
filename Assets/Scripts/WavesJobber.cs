using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Video;

public class WavesJobber : MonoBehaviour
{
    public bool _useJobSystem;
    private MeshFilter MeshFilter;
    [SerializeField] private List<PerlinNoiseLayer> _perlinNoiseLayers;
    
    public int Dimension = 10;
    public float UVScale = 2f;
    
    protected Mesh Mesh;
    void Start()
    {
        //Mesh Setup
        Mesh = new Mesh();
        Mesh.name = gameObject.name;

        Mesh.vertices = GenerateVerts();
        Mesh.triangles = GenerateTries();
        Mesh.uv = GenerateUVs();
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();

        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
        //MeshFilter.mesh.MarkDynamic(); 
    }
    
    void Update()
    {
        var vertices = MeshFilter.mesh.vertices;
        FlattenMesh(vertices);
        if(_useJobSystem)
            ExecJobMethod(vertices);
        else
        {
            foreach (var layer in _perlinNoiseLayers)
            {
                AddPerlinNoise(vertices, layer, Time.timeSinceLevelLoad);
            }
        }
        MeshFilter.mesh.SetVertices(vertices.ToList());
        MeshFilter.mesh.RecalculateNormals();
    }

    private static void AddPerlinNoise(Vector3[] vertices, PerlinNoiseLayer layer, float time)
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            var x = vertices[i].x * layer.Scale + time * layer.Speed;
            var z = vertices[i].z * layer.Scale + time * layer.Speed;

            vertices[i].y += (noise.cnoise(new float2(x,z)) - 0.5f) * layer.Height;
        }
    }

    private void ExecJobMethod(Vector3[] vertices)
    {
        var jobHandles = new List<JobHandle>();
        var vertexArray = new NativeArray<Vector3>(vertices, Allocator.TempJob);

        for (var i = 0; i < _perlinNoiseLayers.Count; i++)
        {
            var job = new WaveJob
            {
                Vertices = vertexArray,
                Layer = _perlinNoiseLayers[i],
                Time = Time.timeSinceLevelLoad
            };
            if(i == 0)
                jobHandles.Add(job.Schedule(vertices.Length,250));
            else
                jobHandles.Add(job.Schedule(vertices.Length,250,jobHandles[i-1]));
        }
        jobHandles.Last().Complete();
        vertexArray.CopyTo(vertices);
        vertexArray.Dispose();

    }

    private static void FlattenMesh(Vector3[] vertices)
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = 0;
        }
    }
    //----------------------
    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        //equaly distributed verts
        for(int x = 0; x <= Dimension; x++)
        for(int z = 0; z <= Dimension; z++)
            verts[index(x, z)] = new Vector3(x, 0, z);

        return verts;
    }

    private int[] GenerateTries()
    {
        var tries = new int[Mesh.vertices.Length * 6];

        //two triangles are one tile
        for(int x = 0; x < Dimension; x++)
        {
            for(int z = 0; z < Dimension; z++)
            {
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x + 1, z + 1);
                tries[index(x, z) * 6 + 2] = index(x + 1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z + 1);
                tries[index(x, z) * 6 + 5] = index(x + 1, z + 1);
            }
        }

        return tries;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[Mesh.vertices.Length];

        //always set one uv over n tiles than flip the uv and set it again
        for (int x = 0; x <= Dimension; x++)
        {
            for (int z = 0; z <= Dimension; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    private int index(int x, int z)
    {
        return x * (Dimension + 1) + z;
    }
}
