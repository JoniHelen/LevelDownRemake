using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelDown/Floor Settings", fileName = "New Floor Settings"), System.Serializable]
public class FloorSettings : ScriptableObject
{
    public float Duration;
}
