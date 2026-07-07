using UnityEngine;
using Battle.Skills;

namespace Handler
{
    public class ProjectileMoveHandler : MonoBehaviour
    {
        private Vector2 _direction;
        private CurrentSkillData _skillData;
        
        public void Init(CurrentSkillData skillData, Transform target, Transform firePoint)
        {
            _skillData = skillData;
            transform.position = firePoint.position;
            
            if (target != null)
                _direction = ((Vector2)target.position - (Vector2)transform.position).normalized;

            Vector3 dir = new Vector3(_direction.x, _direction.y, 0).normalized;

            // 2D 평면(앞쪽)을 바라보는 회전값 계산
            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
        
        private void Update()
        {   
            transform.position += (Vector3)(_direction * _skillData.SkillAbility.Speed / 1000 * Define.BgDistance * Time.deltaTime);
        }
    }
}