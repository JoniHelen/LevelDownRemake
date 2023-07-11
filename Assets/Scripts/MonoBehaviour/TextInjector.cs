using Unity.Entities;
using TMPro;
using LevelDown.Components.Managed;

namespace LevelDown.MonoBehaviour
{
    public class TextInjector : UnityEngine.MonoBehaviour
    {
        public TMP_Text text;

        private void Awake()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            manager.AddComponentObject(manager.CreateEntity(), new TextField { Value = text });
        }
    }
}