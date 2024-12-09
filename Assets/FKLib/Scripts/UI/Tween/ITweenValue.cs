//============================================================
namespace FKLib
{
    internal interface ITweenValue
    {
        void    TweenValue(float floatPercentage);
        bool    IsIgnoreTimeScale { get; }
        float   Duration { get; }
        bool    ValidTarget();
        void    OnFinish();
    }
}
