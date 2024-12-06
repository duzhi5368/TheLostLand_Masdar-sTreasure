//============================================================
namespace FKLib
{
    public enum ENUM_StatModType
    {
        eSMT_Flat,
        eSMT_PercentAdd,
        eSMT_PercentMult,
    }

    public class StatModifier
    {
        public object Source;

        private float _value;
        public float Value { get => _value; }
        private ENUM_StatModType _type;
        public ENUM_StatModType Type { get => _type; }

        public StatModifier() : this(0f, ENUM_StatModType.eSMT_Flat, null) { }
        public StatModifier(float value, ENUM_StatModType type, object source)
        {
            _value = value;
            _type = type;
            Source = source;
        }
    }
}
