using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.VFX;

namespace LevelDown.Components.Managed
{
    public class ParticleComponent : IComponentData, ICloneable, IDisposable
    {
        public VisualEffect Effect;

        public void SendEvent(string name) => Effect.SendEvent(name);
        public void SendEvent(string name, float2 pos)
        {
            Effect.transform.position = new float3(pos, -1);
            Effect.SendEvent(name);
        }

        public void Play(float2 pos)
        {
            Effect.transform.position = new float3(pos, -1);
            Effect.Play();
        }

        public void Stop() => Effect.Stop();

        public object Clone() => new ParticleComponent { Effect = UnityEngine.Object.Instantiate(Effect) };

        public void Dispose() => UnityEngine.Object.Destroy(Effect);
    }
}