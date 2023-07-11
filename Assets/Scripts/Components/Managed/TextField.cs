using TMPro;
using Unity.Entities;

namespace LevelDown.Components.Managed
{
    public class TextField : IComponentData
    {
        public TMP_Text Value;
    }
}