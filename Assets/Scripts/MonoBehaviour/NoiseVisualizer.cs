using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile(FloatPrecision = FloatPrecision.Standard, FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public class NoiseVisualizer : MonoBehaviour
{
    private Renderer _rend;
    private Texture2D _texture;

    [Range(0f, 50f)]
    public float scale;

    private int2 resolution;

    NativeArray<float4> noiseVectors;

    // Start is called before the first frame update
    void Start()
    {
        resolution = new int2(32, 18);

        int size = (int)math.ceil(resolution.x * resolution.y / 4f);

        noiseVectors = new NativeArray<float4>(size, Allocator.Persistent);

        _rend = GetComponent<Renderer>();
        _texture = new(resolution.x, resolution.y);
        _texture.filterMode = FilterMode.Point;
        _rend.sharedMaterial.mainTexture = _texture;
    }

    void Update()
    {
        new VectorizedNoiseGenerationJob {
            ResultNoise = noiseVectors,
            Extents = resolution,
            invHeight = 1f / resolution.y,
            Offset = 0,
            Scale = 3
        }.Schedule(noiseVectors.Length, 8).Complete();

        NativeArray<float> noiseValues = noiseVectors.Reinterpret<float>(16);

        for (int i = 0; i < noiseValues.Length; i++)
        {
            int x = (int)math.floor(1f / resolution.y * i + 0.00001f);
            int y = i - resolution.y * x;

            _texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, (noiseValues[i] + 1) / 2));
        }

        _texture.Apply();
    }

    private void OnDestroy()
    {
        noiseVectors.Dispose();
    }
}