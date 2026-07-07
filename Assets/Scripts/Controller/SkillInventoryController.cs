using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.Player;
using Table.TBL;
using Table.Models;
using UnityEngine;
using Managers;
using static Managers.DataTableManager;

namespace Battle.Skills
{
    public class SkillInventoryController : MonoBehaviour
    {
        [Space(10)] [Header("테스트용")] [SerializeField]
        PlayerConfigSO _playerInfoSo;

        private readonly List<CurrentSkillData> _skills = new();
        public List<CurrentSkillData> CurrentSkills => _skills;
        public IReadOnlyCollection<int> CurrentSkillsRows => _skills.Select(x => x.SkillRow.No).ToList();
        public IReadOnlyCollection<int> CurrentSKillUpgradeRows => _skills.SelectMany(x => x.SkillUpgradeRows).ToList();

        // 초기 설정
        public async UniTask InitDefaultProjectileAsync()
        {
            if (_playerInfoSo == null)
                return;

            PlayerSkillRow defaultSkillRow = GetTable<PlayerSkillTable>()?.GetData(_playerInfoSo.PlayerData.No);
            if (defaultSkillRow != null)
            {
                CurrentSkillData defaultWeapon = new CurrentSkillData(defaultSkillRow);
                await defaultWeapon.InitWeaponPoolsAsync();
                _skills.Add(defaultWeapon);
                GameEvents.SendEvent(new PlayerSkillEquippedEvent(defaultSkillRow.No));
            }
        }

        public async UniTask ApplySelectedSkillAsync(PlayerSkillRow skillRow, PlayerSkillUpgradeRow upgradeRow, Action cb)
        {
            await ApplySkillUpgradeAsync(skillRow, upgradeRow);
            cb?.Invoke();
        }

        private async UniTask ApplySkillUpgradeAsync(PlayerSkillRow skillRow, PlayerSkillUpgradeRow upgradeRow)
        {
            if (skillRow is null)
                return;

            if (upgradeRow is null)
            {
                await AddNewSkillAsync(skillRow);
                return;
            }

            ApplyUpgradeSkill(skillRow, upgradeRow);
        }

        private async UniTask AddNewSkillAsync(PlayerSkillRow skillRow)
        {
            if (HasSkill(skillRow))
            {
                DebugUtils.LogError($"Already have Skills: {skillRow.No}");
                return;
            }

            var skillData = new CurrentSkillData(skillRow);

            await skillData.InitWeaponPoolsAsync();
            _skills.Add(skillData);

            GameEvents.SendEvent(new PlayerSkillEquippedEvent(skillData.SkillRow.No));
        }

        private void ApplyUpgradeSkill(PlayerSkillRow skillRow, PlayerSkillUpgradeRow upgradeRow)
        {
            var skillData = FindCurrentSkill(skillRow);

            if (skillData is null)
            {
                DebugUtils.LogError($"Base Skill Not Found: {skillRow.No}");
                return;
            }

            skillData.ReplaceUpgradeSkill(skillRow, upgradeRow);
        }

        private bool HasSkill(PlayerSkillRow skillRow)
        {
            return skillRow != null && _skills.Any(skill => skill.SkillRow.No == skillRow.No);
        }

        private CurrentSkillData FindCurrentSkill(PlayerSkillRow skillRow)
        {
            if (skillRow == null)
                return null;

            return _skills.FirstOrDefault(skill => skill.SkillRow.No == skillRow.No);
        }
    }
}