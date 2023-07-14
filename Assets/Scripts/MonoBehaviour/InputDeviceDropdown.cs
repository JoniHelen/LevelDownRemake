using TMPro;
using Unity.Entities;
using LevelDown.Components.Managed;

namespace LevelDown.MonoBehaviour
{
    public class InputDeviceDropdown : UnityEngine.MonoBehaviour
    {
        public TMP_Dropdown dropdown;

        private void Awake()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            manager.CreateSingleton(new Dropdown { Value = dropdown });
        }
    }
}