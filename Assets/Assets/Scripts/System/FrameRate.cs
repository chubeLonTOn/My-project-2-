using UnityEngine;

namespace Assets.Scripts.System
{
    public static class FrameRateLimiter
    {
        public const int limit = 120;

        [RuntimeInitializeOnLoadMethod]
        public static void Limiter()
        {
            Application.targetFrameRate = limit;
        }
    }
}
