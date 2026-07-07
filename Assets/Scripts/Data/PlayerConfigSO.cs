using UnityEngine;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerConfigSO", menuName = "Scriptable Objects/PlayerConfigSO")]
public class PlayerConfigSO : ScriptableObject
{
    [Header("▼ 플레이어 데이터 설정")]
    public int WallHP;
    
    public PlayerDataConfig PlayerData; 
    
    [Header("▼ 스킬 선택")]
    public int selectedSkillNo;
    
    private Table.TBL.PlayerSkillTable _table;

    private void OnEnable()
    {
        if (_table == null)
        {
            _table = Managers.DataTableManager.GetEditorTable<Table.TBL.PlayerSkillTable>();
        }
    }

    public Table.TBL.PlayerSkillTable GetTable()
    {
        if (_table == null)
        {
            _table = Managers.DataTableManager.GetEditorTable<Table.TBL.PlayerSkillTable>();
        }
        return _table;
    }

    // 인스펙터 에디터에서 강제로 트리거할 자동 매핑 핵심 함수
    public void UpdateSelectedSkillData()
    {
        var table = GetTable();
        if (table == null) return;

        var row = table.GetData(selectedSkillNo);
        if (row == null) return;

        PlayerData.No = row.No;
        PlayerData.Damage = row.Skill_Damage;
        PlayerData.Speed = row.Skill_Speed;
        PlayerData.Delay = row.Cooldown;
        PlayerData.Prefab = row.Prefab; // 누락되었던 프리팹 이름 대입 추가
        
#if UNITY_EDITOR
        PlayerData.ProjectilePrefab = FindAssetByName<GameObject>(row.Prefab, "t:prefab", "Assets/AddressableResource/Projectile");
        PlayerData.Fx = FindAssetByName<GameObject>(row.Fx, "t:prefab", "Assets/AddressableResource/Fx");
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    
#if UNITY_EDITOR
    private T FindAssetByName<T>(string assetName, string filterType, string searchFolder = null) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetName) || assetName == "NONE") return null;

        // 확장자나 경로가 섞여 들어와도 순수 파일명만 추출
        string pureAssetName = System.IO.Path.GetFileNameWithoutExtension(assetName);
        string searchQuery = $"{pureAssetName} {filterType}";

        string[] guids;

        // 💡 특정 폴더 경로가 지정되었다면 해당 폴더 안에서만 찾습니다.
        if (!string.IsNullOrEmpty(searchFolder))
        {
            // 폴더 경로는 배열 형태로 넘겨야 합니다. 예: new[] { "Assets/AddressableResource/Projectile" }
            guids = UnityEditor.AssetDatabase.FindAssets(searchQuery, new[] { searchFolder });
        }
        else
        {
            // 폴더 지정이 없으면 프로젝트 전체 검색
            guids = UnityEditor.AssetDatabase.FindAssets(searchQuery);
        }

        if (guids != null && guids.Length > 0)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        Debug.LogWarning($"[에셋 자동 세팅 실패] 경로({searchFolder}) 내에서 '{pureAssetName}'을 찾을 수 없습니다.");
        return null;
    }
#endif
}

[Serializable]
public class PlayerDataConfig
{
 
    public GameObject ProjectilePrefab;
    public int No;
    public string Prefab;
    public int Damage;
    public int Speed;
    public float Delay;
    public GameObject Fx;
}