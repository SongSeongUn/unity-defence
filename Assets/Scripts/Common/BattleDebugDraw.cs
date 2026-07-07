using UnityEngine;

namespace Common
{
    public static class BattleDebugDraw
    {
#if UNITY_EDITOR
        public static Vector2 ExplosionPos;
        public static float ExplosionRadius;
        public static float EndTime;

        public static void DrawExplosionRadius(Vector2 pos, float radius, float duration = 0.6f)
        {
            ExplosionPos = pos;
            ExplosionRadius = radius;
            EndTime = Time.time + duration;
        }
#endif
    }
    

}