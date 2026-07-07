using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Events;
using Events.Player;
using Table.Models;
using Table.TBL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Managers.DataTableManager;

namespace UI.UIItem
{
    public class UI_SkillSelectSlot : MonoBehaviour
    {
        [SerializeField] private Image _skillIcon;
        [SerializeField] private TextExtension _titleText;
        [SerializeField] private TextExtension _detailText;
        [SerializeField] private TextExtension _levelBeforeText;
        [SerializeField] private TextExtension _levelAfterText;
        [SerializeField] private GameObject _textParent;
        [SerializeField] private Button _button;

        private PlayerSkillUpgradeRow _skillUpgradeRow;
        private PlayerSkillRow _playerSkillRow;

        private PlayerSkillTable _playerSkillTable;
        private PlayerSKillUpgradeTable _playerSKillUpgradeTable;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
            _skillUpgradeRow = null;
            _playerSkillRow = null;
        }

        private void Awake()
        {
            _playerSkillTable = GetTable<PlayerSkillTable>();
            _playerSKillUpgradeTable =  GetTable<PlayerSKillUpgradeTable>();
            
            if (_playerSkillTable == null)
                DebugUtils.LogError("PlayerSkillTable is null");
            
            if (_playerSKillUpgradeTable == null)
                DebugUtils.LogError("PlayerSKillUpgradeTable is null");
        }

        private void Init(int playerSkillNo)
        {
            _playerSkillRow = _playerSkillTable.GetData(playerSkillNo);
            
            if (_playerSkillRow == null)
            {
                DebugUtils.LogError($"{playerSkillNo} playerSkillNo is null");
                return;
            }
            LoadIconAsync(_playerSkillRow?.Skill_Icon).Forget();
        }
        

        public void ShowNewSkill(int playerSkillNo)
        {
            Init(playerSkillNo);
            ShowInfo(false);
            
            EnableNewSkillActive(true);
            gameObject.SetActive(true);
        }

        public void ShowUpgradeSkill(int battleSkillUpgradeNo)
        {
            _skillUpgradeRow = _playerSKillUpgradeTable.GetData(battleSkillUpgradeNo);

            if (_skillUpgradeRow == null)
            {
                DebugUtils.LogError($"{battleSkillUpgradeNo} SkillUpgradeRow is null");
                return;
            }
            
            Init(_skillUpgradeRow.Skill_No);
            ShowInfo(true);
            
            EnableNewSkillActive(false);
            gameObject.SetActive(true);
        }

        private void ShowInfo(bool isUpgrade)
        {
            _titleText.SetTextSiblingIndex(_playerSkillRow.Name);

            if (isUpgrade)
            {
                var firstRow = _playerSKillUpgradeTable.GetPlayerSkillUpgradeFirstRowData(_skillUpgradeRow);
                if (firstRow is null)
                {
                    DebugUtils.LogError("첫 번째 스킬 데이터 못찾음");
                    return;
                }

                int beforeLevel = _skillUpgradeRow.No - firstRow.No;
                int afterLevel = beforeLevel + 1;

                _levelBeforeText.TextValue = $"Lv.{beforeLevel}";
                _levelAfterText.TextValue = $"Lv.{afterLevel}";

                switch (EnumParser.ParseUpgradeType(_skillUpgradeRow.SKILL_UPGRADE_TYPE))
                {
                    case SkillUpgradeType.DamageUpgrade:
                        int addDamageRate = _skillUpgradeRow.Skill_Damage_Add_Rate / 1000;
                        int addEffectDamageRate = _skillUpgradeRow.Effect_Damage_Add_Rate / 1000;
                        if (EnumParser.ParseHitType(_playerSkillRow.Hit_Type) == SkillHitType.Chain)
                        {
                            _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, addEffectDamageRate, 0);
                        }
                        else
                        {
                            _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, addDamageRate,
                                addEffectDamageRate);
                        }
                        break;

                    case SkillUpgradeType.SpawnUpgrade:
                        int spawn = _skillUpgradeRow.Skill_Add_Count;
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, spawn);
                        break;

                    case SkillUpgradeType.HitCountUpgrade:
                        int hit = _skillUpgradeRow.Effect_Add_Count;
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, hit);
                        break;

                    case SkillUpgradeType.CooldownUpgrade:
                        int cooldown = _skillUpgradeRow.Cooldown_Sub_Rate / 1000;
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, cooldown);
                        break;

                    case SkillUpgradeType.PierceUpgrade:
                        int statRate = _skillUpgradeRow.Stat_Add_Rate * beforeLevel / 1000; // 현재 확률
                        int addStatRate = _skillUpgradeRow.Stat_Add_Rate / 1000; // 추가 확률
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, statRate, addStatRate);
                        break;

                    case SkillUpgradeType.RangeUpgrade:
                        int addRangeRate = _skillUpgradeRow.Stat_Add_Rate / 10;
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, addRangeRate);
                        break;

                    case SkillUpgradeType.CCUpgrade:
                        int ccRate = (_playerSkillRow.CC_Rate + _skillUpgradeRow.Stat_Add_Rate * beforeLevel) /
                                     1000; // 현재 확률
                        int addCcRate = _skillUpgradeRow.Stat_Add_Rate / 1000; // 추가 확률
                        _detailText.SetTextSiblingIndex(_skillUpgradeRow.Description, ccRate, addCcRate);
                        break;
                }
            }
            else
            {
                if (EnumParser.ParseHitType(_playerSkillRow.Hit_Type) == SkillHitType.Chain)
                    _detailText.SetTextSiblingIndex(_playerSkillRow.Description, _playerSkillRow.Skill_Count + _playerSkillRow.Effect_Count);
                else
                    _detailText.SetTextSiblingIndex(_playerSkillRow.Description);
            }
        }

        private void EnableNewSkillActive(bool isNew)
        {
            _textParent.SetActive(!isNew);
            _levelBeforeText.gameObject.SetActive(!isNew);
            _levelAfterText.gameObject.SetActive(!isNew);
        }

        private void OnClick()
        {
            if (_skillUpgradeRow == null && _playerSkillRow == null)
                return;
            
            GameEvents.SendEvent(new PlayerSkillSelecdtEvent(_playerSkillRow, _skillUpgradeRow));
        }

        private async UniTaskVoid LoadIconAsync(string icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
                return;

            Sprite sprite =
                await AddressableManager.Instance.LoadAssetAsync<Sprite>(
                    $"Resource/Skill/{icon}.png");

            if (sprite != null)
            {
                _skillIcon.sprite = sprite;
                _skillIcon.enabled = true;
            }
        }
    }
}