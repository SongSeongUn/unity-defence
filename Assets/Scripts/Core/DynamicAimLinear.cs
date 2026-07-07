using UnityEngine;
using Actor;
using System.Collections.Generic;

namespace Core
{
    // 타겟까지 라인 길이 계산
    public static class DynamicAimLinear
    {
       /// <summary>
        /// 시작점부터 끝점까지 정해진 단위 길이마다의 좌표들을 반환합니다.
        /// </summary>
        public static List<Vector3> GetSplitPoints(Vector3 start, Vector3 end, float maxLength)
        {
            List<Vector3> resultPoints = new List<Vector3>();

            float totalLength = (end - start).magnitude; // 벡터 총 길이
            Vector3 dir = (end - start).normalized; // 방향

            // maxLength씩 더해가며 좌표 계산
            for (float currentDistance = maxLength; currentDistance <= totalLength; currentDistance += maxLength)
            {
                // 시작점 + (방향 * 거리)
                Vector3 newPoint = start + (dir * currentDistance);
                resultPoints.Add(newPoint);
            }
            
            // 2. 만약 마지막으로 추가된 좌표가 실제 끝점(end)이 아니라면, 
            // 혹은 전체 길이가 단위 길이로 딱 나누어 떨어지지 않아 자투리가 남았다면 끝점을 강제로 추가
            if (resultPoints.Count == 0 || Vector3.Distance(resultPoints[resultPoints.Count - 1], end) > 0.01f)
            {
                resultPoints.Add(end);
            }

            return resultPoints;
        }
    }
}