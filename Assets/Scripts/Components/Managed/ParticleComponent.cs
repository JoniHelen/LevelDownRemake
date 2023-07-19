using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

namespace LevelDown.Components.Managed
{
    public class ParticleComponent : IComponentData, ICloneable, IDisposable
    {
        public GameObject Obj;

        private VisualEffect _effect;
        public VisualEffect Effect { 
            get 
            {
                if (_effect == null)
                    _effect = Obj.GetComponent<VisualEffect>();

                return _effect;
            } 
        }

        public void SendEvent(string name) => Effect.SendEvent(name);
        public void SendEvent(string name, float2 pos)
        {
            Obj.transform.position = new float3(pos, -1);
            Effect.SendEvent(name);
        }

        public void Play(float2 pos)
        {
            Obj.transform.position = new float3(pos, -1);
            Effect.Play();
        }

        public void Stop() => Effect.Stop();

        public object Clone() => new ParticleComponent { Obj = UnityEngine.Object.Instantiate(Obj) };

        public void Dispose() => UnityEngine.Object.DestroyImmediate(Obj);
    }
}