using UnityEngine;

namespace LevelDown.ScriptableObjects
{
    [CreateAssetMenu(menuName = "LevelDown/Floor Settings", fileName = "New Floor Settings"), System.Serializable]
    public class FloorSettings : ScriptableObject
    {
        public float Duration;
    }
}