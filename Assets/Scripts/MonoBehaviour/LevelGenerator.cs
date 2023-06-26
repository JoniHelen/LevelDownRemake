using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Entities.Hybrid;
using Unity.Entities.Serialization;
using Unity.Collections;

public class LevelGenerator : MonoBehaviour
{
    private World _World;
    public GameObject _Floor;

    void Start()
    {
        _World = World.DefaultGameObjectInjectionWorld;

        Entity BufferEntity = _World.EntityManager.CreateSingletonBuffer<EntityDynamicBufferComponent>("Floor Buffer");

        var prefab = _World.EntityManager.CreateEntityQuery(typeof(EntityPrefabComponent))
            .GetSingleton<EntityPrefabComponent>();

        for (int i = 0; i < 1152; i++)
        {
            Entity instance = _World.EntityManager.Instantiate(prefab.Value);
            _World.EntityManager.SetEnabled(instance, false);

            _World.EntityManager.GetBuffer<EntityDynamicBufferComponent>(
            BufferEntity).Add(new EntityDynamicBufferComponent
            {
                entity = instance
            });
        }
    }
}
