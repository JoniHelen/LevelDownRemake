using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Jobs;
using LevelDown.Components.Singletons;
using LevelDown.Noise;
using LevelDown.Noise.Vectorized;

public class NoiseVisualizer : MonoBehaviour
{
    public GameObject quad1;
    public GameObject quad2;
    public GameObject quad3;

    private Renderer _rend;
    private Texture2D _texture;

    private Renderer _rend2;
    private Texture2D _texture2;

    private Renderer _rend3;
    private Texture2D _texture3;

    [Range(0f, 50f)]
    public float Scale;

    private const float Threshold = 0.6f;

    private int2 resolution;

    NativeArray<float4> noiseVectors;

    private World world;

    public float2 PlayerPos;

    private Color32[] clearColors;

    // Start is called before the first frame update
    void Start()
    {
        resolution = new int2(32, 18);

        int size = (int)math.ceil(resolution.x * resolution.y / 4f);

        noiseVectors = new NativeArray<float4>(size, Allocator.Persistent);

        _rend = quad1.GetComponent<Renderer>();
        _texture = new(resolution.x, resolution.y);
        _texture.filterMode = FilterMode.Point;
        _rend.sharedMaterial.mainTexture = _texture;

        _rend2 = quad2.GetComponent<Renderer>();
        _texture2 = new(resolution.x, resolution.y);
        _texture2.filterMode = FilterMode.Point;
        _rend2.sharedMaterial.mainTexture = _texture2;

        _rend3 = quad3.GetComponent<Renderer>();
        _texture3 = new(resolution.x, resolution.y);
        _texture3.filterMode = FilterMode.Point;
        _rend3.sharedMaterial.mainTexture = _texture3;

        world = World.DefaultGameObjectInjectionWorld;

        clearColors = new Color32[576];
        System.Array.Fill(clearColors, new Color32(255, 255, 255, 255));
    }

    void Update()
    {
        NativeReference<float2> offset = new(Allocator.TempJob);

        float2 local = 0;

        /*if (world.EntityManager.CreateEntityQuery(typeof(TempPlayerPos)).TryGetSingleton(out TempPlayerPos pos))
            local = pos.Value;*/

        float2 playerPosition = math.floor(local + new float2(resolution.x / 2f - 0.5f, resolution.y / 2f - 0.5f));

        _texture3.SetPixels32(clearColors);
        _texture3.SetPixel((int)playerPosition.x, (int)playerPosition.y, Color.red);
        _texture3.Apply();

        var texelSize = 1f / (float2)resolution;

        for (var i = 0; i < 100; i++)
        {
            if (i == 99) Debug.Log($"OFFSET FAILED WITH OFFSET: {offset}, POSITION: {playerPosition}, TEXELSIZE: {texelSize}");

            if (PlayerOverlapsLevel(playerPosition, offset.Value, texelSize))
                offset.Value += texelSize;
            else
                break;
        }

        new VectorizedNoiseGenerationJob {
            ResultNoise = noiseVectors,
            Extents = resolution,
            InvHeight = 1f / resolution.y,
            Offset = offset,
            Scale = Scale
        }.Schedule(noiseVectors.Length, 8).Complete();

        NativeArray<float> noiseValues = noiseVectors.Reinterpret<float>(16);

        for (int i = 0; i < noiseValues.Length; i++)
        {
            int x = (int)math.floor(1f / resolution.y * i + 0.00001f);
            int y = i - resolution.y * x;

            _texture.SetPixel(x, y, (noiseValues[i] + 1) / 2 >= Threshold ? Color.white : Color.black);

            // Color.Lerp(Color.black, Color.white, (noiseValues[i] + 1) / 2)
        }

        _texture.Apply();
    }

    private bool PlayerOverlapsLevel(float2 position, float2 offset, float2 texelSize)
    {
        float normalizedTexelSize = texelSize.x > texelSize.y ? texelSize.x : texelSize.y;
        var playerTexel = position * normalizedTexelSize + offset;

        if ((Simplex.GetNoise(playerTexel, Scale) + 1) / 2 >= Threshold) return true;

        var u1 = new float4
        {
            x = playerTexel.x - normalizedTexelSize,
            y = playerTexel.x - normalizedTexelSize,
            z = playerTexel.x,
            w = playerTexel.x + normalizedTexelSize
        };

        var u2 = new float4
        {
            x = playerTexel.x + normalizedTexelSize,
            y = playerTexel.x + normalizedTexelSize,
            z = playerTexel.x,
            w = playerTexel.x - normalizedTexelSize
        };

        var v1 = new float4
        {
            x = playerTexel.y,
            y = playerTexel.y + normalizedTexelSize,
            z = playerTexel.y + normalizedTexelSize,
            w = playerTexel.y + normalizedTexelSize
        };

        var v2 = new float4
        {
            x = playerTexel.y,
            y = playerTexel.y - normalizedTexelSize,
            z = playerTexel.y - normalizedTexelSize,
            w = playerTexel.y - normalizedTexelSize
        };

        NativeArray<float4> values = new(2, Allocator.Temp);
        values[0] = Simplex4.GetNoise4(new float4x2(u1, v1), Scale);
        values[1] = Simplex4.GetNoise4(new float4x2(u2, v2), Scale);

        var noise = values.Reinterpret<float>(sizeof(float) * 4);

        _texture2.SetPixels32(clearColors);

        for (int i = 0; i < noise.Length; i++)
        {
            int2 dir = i switch
            {
                0 => new(-1, 0),
                1 => new(-1, 1),
                2 => new(0, 1),
                3 => new(1, 1),
                4 => new(1, 0),
                5 => new(1, -1),
                6 => new(0, -1),
                7 => new(-1, -1),
                _ => new(0, 0)
            };

            _texture2.SetPixel((int)position.x + dir.x, (int)position.y + dir.y, (noise[i] + 1) / 2 >= Threshold ? Color.white : Color.black);
        }

        _texture2.Apply();

        foreach (var value in values.Reinterpret<float>(sizeof(float) * 4))
            if ((value + 1) / 2 >= Threshold)
            {
                values.Dispose();
                return true;
            }

        values.Dispose();
        return false;
    }

    private void OnDestroy()
    {
        noiseVectors.Dispose();
    }
}