using UnityEngine;
namespace Common
{
    public class BattleDebugGizmoDrawer : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (Time.time > BattleDebugDraw.EndTime)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                BattleDebugDraw.ExplosionPos,
                BattleDebugDraw.ExplosionRadius
            );
        }
#endif
    }
}