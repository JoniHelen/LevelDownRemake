namespace LevelDown.Input
{
    public struct ProjectileData
    {
        private byte _data;

        public ProjectileData(byte data = 0) => _data = data;
        public ProjectileData(ProjectileProperty data = 0) => _data = (byte)data;

        public static implicit operator byte(ProjectileData data) => data._data;
        public static implicit operator ProjectileData(byte data) => new(data);

        public static implicit operator ProjectileProperty(ProjectileData data) => (ProjectileProperty)data._data;
        public static implicit operator ProjectileData(ProjectileProperty data) => new(data);

        public bool IsSet(int index) => (_data & (1 << index)) != 0;
        public bool IsSet(ProjectileProperty property) => (_data & (byte)property) != 0;
        public bool IsSet(byte data) => (_data & data) != 0;

        public void SetBit(int index, bool value)
            => _data = (byte)(value ? (_data | (1 << index)) : (_data & ~(1 << index)));
        public void SetBit(ProjectileProperty property, bool value)
            => _data = (byte)(value ? (_data | (byte)property) : (_data & ~(byte)property));
        public void SetBit(byte data, bool value)
            => _data = (byte)(value ? (_data | data) : (_data & ~data));

        public bool this[int index]
        {
            get => IsSet(index);
            set => SetBit(index, value);
        }

        public bool this[byte data]
        {
            get => IsSet(data);
            set => SetBit(data, value);
        }

        public bool this[ProjectileProperty property]
        {
            get => IsSet(property);
            set => SetBit(property, value);
        }
    }

    [System.Flags]
    public enum ProjectileProperty : byte
    {
        None = 0,
        Explosive = 1,
        Seeking = 1 << 1,
        Piercing = 1 << 2,
        Ricocheting = 1 << 3,
        All = 0b1111
    }
}