using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
public struct WaveJob : IJobParallelFor
{
    public NativeArray<Vector3> Vertices;
    public PerlinNoiseLayer Layer;
    public float Time;
    public void Execute(int i )
    {
        var vertex = Vertices[i];

        var x = vertex.x * Layer.Scale + Time * Layer.Speed;
        var z = vertex.z * Layer.Scale + Time * Layer.Speed;

       vertex.y += (noise.cnoise(new float2(x,z)) - 0.5f) * Layer.Height;


        Vertices[i] = vertex;
    }
}
