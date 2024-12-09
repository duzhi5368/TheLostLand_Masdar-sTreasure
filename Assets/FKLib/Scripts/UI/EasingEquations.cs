using UnityEngine;
//============================================================
namespace FKLib
{
    public static class EasingEquations
    {
        public enum ENUM_EaseType
        {
            eET_EaseInQuad,
            eET_EaseOutQuad,
            eET_EaseInOutQuad,
            eET_EaseInCubic,
            eET_EaseOutCubic,
            eET_EaseInOutCubic,
            eET_EaseInQuart,
            eET_EaseOutQuart,
            eET_EaseInOutQuart,
            eET_EaseInQuint,
            eET_EaseOutQuint,
            eET_EaseInOutQuint,
            eET_EaseInSine,
            eET_EaseOutSine,
            eET_EaseInOutSine,
            eET_EaseInExpo,
            eET_EaseOutExpo,
            eET_EaseInOutExpo,
            eET_EaseInCirc,
            eET_EaseOutCirc,
            eET_EaseInOutCirc,
            eET_Linear,
            eET_Spring,
            eET_EaseInBounce,
            eET_EaseOutBounce,
            eET_EaseInOutBounce,
            eET_EaseInBack,
            eET_EaseOutBack,
            eET_EaseInOutBack,
            eET_EaseInElastic,
            eET_EaseOutElastic,
            eET_EaseInOutElastic,
            eET_Punch,
        }

        public static float GetValue(ENUM_EaseType easeType, float start, float end, float value)
        {
            switch (easeType)
            {
                case ENUM_EaseType.eET_EaseInQuad:
                    return EaseInQuad(start, end, value);
                case ENUM_EaseType.eET_EaseOutQuad:
                    return EaseOutQuad(start, end, value);
                case ENUM_EaseType.eET_EaseInOutQuad:
                    return EaseInOutQuad(start, end, value);
                case ENUM_EaseType.eET_EaseInCubic:
                    return EaseInCubic(start, end, value);
                case ENUM_EaseType.eET_EaseOutCubic:
                    return EaseOutCubic(start, end, value);
                case ENUM_EaseType.eET_EaseInOutCubic:
                    return EaseInOutCubic(start, end, value);
                case ENUM_EaseType.eET_EaseInQuart:
                    return EaseInQuart(start, end, value);
                case ENUM_EaseType.eET_EaseOutQuart:
                    return EaseOutQuart(start, end, value);
                case ENUM_EaseType.eET_EaseInOutQuart:
                    return EaseInOutQuart(start, end, value);
                case ENUM_EaseType.eET_EaseInQuint:
                    return EaseInQuint(start, end, value);
                case ENUM_EaseType.eET_EaseOutQuint:
                    return EaseOutQuint(start, end, value);
                case ENUM_EaseType.eET_EaseInOutQuint:
                    return EaseInOutQuint(start, end, value);
                case ENUM_EaseType.eET_EaseInSine:
                    return EaseInSine(start, end, value);
                case ENUM_EaseType.eET_EaseOutSine:
                    return EaseOutSine(start, end, value);
                case ENUM_EaseType.eET_EaseInOutSine:
                    return EaseInOutSine(start, end, value);
                case ENUM_EaseType.eET_EaseInExpo:
                    return EaseInExpo(start, end, value);
                case ENUM_EaseType.eET_EaseOutExpo:
                    return EaseOutExpo(start, end, value);
                case ENUM_EaseType.eET_EaseInOutExpo:
                    return EaseInOutExpo(start, end, value);
                case ENUM_EaseType.eET_EaseInCirc:
                    return EaseInCirc(start, end, value);
                case ENUM_EaseType.eET_EaseOutCirc:
                    return EaseOutCirc(start, end, value);
                case ENUM_EaseType.eET_EaseInOutCirc:
                    return EaseInOutCirc(start, end, value);
                case ENUM_EaseType.eET_Spring:
                    return Spring(start, end, value);
                case ENUM_EaseType.eET_EaseInBounce:
                    return EaseInBounce(start, end, value);
                case ENUM_EaseType.eET_EaseOutBounce:
                    return EaseOutBounce(start, end, value);
                case ENUM_EaseType.eET_EaseInOutBounce:
                    return EaseInOutBounce(start, end, value);
                case ENUM_EaseType.eET_EaseInBack:
                    return EaseInBack(start, end, value);
                case ENUM_EaseType.eET_EaseOutBack:
                    return EaseOutBack(start, end, value);
                case ENUM_EaseType.eET_EaseInOutBack:
                    return EaseInOutBack(start, end, value);
                case ENUM_EaseType.eET_EaseInElastic:
                    return EaseInElastic(start, end, value);
                case ENUM_EaseType.eET_EaseOutElastic:
                    return EaseOutElastic(start, end, value);
                case ENUM_EaseType.eET_EaseInOutElastic:
                    return EaseInOutElastic(start, end, value);
                default:
                    return Linear(start, end, value);
            }
        }

        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        public static float CLerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) * 0.5f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            return retval;
        }

        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value + start;
            value -= 2;
            return -end * 0.5f * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }

        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d * 0.5f) return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525f);
            return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float Punch(float amplitude, float value)
        {
            float s = 9;
            if (value == 0)
            {
                return 0;
            }
            else if (value == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        public static float EaseOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d * 0.5f) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }
}
