using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Editor
{
    [CustomEditor(typeof(PlayerConfigSO))]
    public class PlayerConfigSOEditor : UnityEditor.Editor
    {
        private PlayerConfigSO _targetSO;
        private string[] _dropdownDisplayNames;
        private int[] _dropdownValues;
        private int _currentSelectedIndex = 0;

        private void OnEnable()
        {
            _targetSO = (PlayerConfigSO)target;
            SetupDropdownData();
        }


        private void SetupDropdownData()
        {
            var table = _targetSO.GetTable();
            if (table == null || table.Rows == null) return;

            var rows = table.Rows;
            int count = rows.Count;

            _dropdownDisplayNames = new string[count];
            _dropdownValues = new int[count];

            for (int i = 0; i < count; i++)
            {
                var row = rows[i];
                _dropdownDisplayNames[i] = $"[{row.No}] {row.Prefab}";
                _dropdownValues[i] = row.No;

                // 현재 SO에 저장되어 있던 기선택된 스킬 번호와 매칭되는 인덱스 찾기
                if (row.No == _targetSO.selectedSkillNo)
                {
                    _currentSelectedIndex = i;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            // 데이터 실시간 동기화 시작
            serializedObject.Update();

            // --- 1. 플레이어 데이터 설정 구역 ---
            GUILayout.Label("플레이어 데이터 설정", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                _targetSO.WallHP = EditorGUILayout.IntField("WallHP", _targetSO.WallHP);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // --- 2. 스킬 드롭다운 구역 (오딘 ValueDropdown + OnValueChanged 대체) ---
            GUILayout.Label("스킬 선택 데이터 파싱", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                if (_dropdownDisplayNames != null && _dropdownDisplayNames.Length > 0)
                {
                    int lastIndex = _currentSelectedIndex;
                    _currentSelectedIndex = EditorGUILayout.Popup("스킬 선택", _currentSelectedIndex, _dropdownDisplayNames);

                    // 🌟 값이 바뀌었을 때 실행되는 체인 (오딘 [OnValueChanged] 재현)
                    if (lastIndex != _currentSelectedIndex || _targetSO.selectedSkillNo != _dropdownValues[_currentSelectedIndex])
                    {
                        _targetSO.selectedSkillNo = _dropdownValues[_currentSelectedIndex];
                        _targetSO.UpdateSelectedSkillData(); // 내부 자동 변환 함수 호출!
                        EditorUtility.SetDirty(_targetSO);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("데이터 테이블을 로드할 수 없거나 비어 있습니다. 게임 툴을 확인하세요.", MessageType.Warning);
                    // 재시도 캐싱 용도
                    if (GUILayout.Button("테이블 다시 불러오기")) SetupDropdownData();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // --- 3. PlayerDataConfig 수평/수직 레이아웃 칼각 정렬 (오딘 PreviewField + ReadOnly 대체) ---
            GUILayout.Label("Player Data (결과 정보 프리뷰)", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var data = _targetSO.PlayerData;

                EditorGUILayout.BeginHorizontal();
                {
                    // 왼쪽 영역: 썸네일 미리보기 (오딘 [PreviewField] 구현)
                    Texture2D previewTex = AssetPreview.GetAssetPreview(data.ProjectilePrefab);
                    if (previewTex != null)
                    {
                        GUILayout.Label(previewTex, GUILayout.Width(65), GUILayout.Height(65));
                    }
                    else
                    {
                        // 프리뷰가 로드 안 되었을 때는 빈 사각형 더미 배치
                        Rect rect = GUILayoutUtility.GetRect(65, 65, GUILayout.ExpandWidth(false));
                        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
                        GUI.Label(new Rect(rect.x + 10, rect.y + 25, 50, 20), "No Image", EditorStyles.miniLabel);
                    }

                    EditorGUILayout.Space(5);

                    // 오른쪽 영역: 세로 정보 나열 (오딘 ReadOnly 라벨 셋팅)
                    EditorGUILayout.BeginVertical();
                    {
                        GUI.enabled = false; // 🌟 전체 필드 [ReadOnly] 처리 활성화
                        
                        data.ProjectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile", data.ProjectilePrefab, typeof(GameObject), false);
                        data.No = EditorGUILayout.IntField("Weapon No", data.No);
                        data.Prefab = EditorGUILayout.TextField("Weapon Name", data.Prefab);
                        data.Damage = EditorGUILayout.IntField("Damage", data.Damage);
                        data.Speed = EditorGUILayout.IntField("Speed", data.Speed);
                        data.Delay = EditorGUILayout.FloatField("Delay", data.Delay);
                        data.Fx = (GameObject)EditorGUILayout.ObjectField("Fx", data.Fx, typeof(GameObject), false);
                        
                        GUI.enabled = true; // ReadOnly 해제
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 수정한 내용 데이터 구조 파일에 강제 저장 유도
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_targetSO);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}