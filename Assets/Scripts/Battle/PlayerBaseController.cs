using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Events;
using Events.Player;
using Table.Models;
using UnityEngine;
using Managers;
using Table.TBL;
using static Managers.DataTableManager;

namespace Battle
{
    [Obsolete]
    public class PlayerBaseController : MonoBehaviour
    {
        [SerializeField] private PlayerAttackHandler _playerAttackHandler;
        [SerializeField] private WallActor _wallActor;
        public WallActor WallActor => _wallActor;

        private UI.Popup.UI_SkillSelect_Popup _uiSelectPopup; 
    }
}