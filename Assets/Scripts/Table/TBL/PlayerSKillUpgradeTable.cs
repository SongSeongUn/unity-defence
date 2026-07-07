using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;


namespace Table.TBL
{
    public class PlayerSkillUpgradeGroup
    {
        public int FirstRowNo => Rows.First().No;
        public int SkillNo => Rows.First().Skill_No;
        public List<PlayerSkillUpgradeRow> Rows = new ();
    }
    
    public class PlayerSKillUpgradeTable : TableReader, ITableInterface<PlayerSkillUpgradeRow>
    {
        private TableRepository<PlayerSkillUpgradeRow> _repository = new(new List<PlayerSkillUpgradeRow>());

        public IReadOnlyList<PlayerSkillUpgradeRow> Rows => _repository.Rows;

        public List<PlayerSkillUpgradeGroup> GroupRows { get; private set; } = new();

        public bool TryGet(int no, out PlayerSkillUpgradeRow row) => _repository.TryGet(no, out row);

        public PlayerSkillUpgradeRow GetData(int no)
        {
            if (TryGet(no, out var row))
                return row;

            return null;
        }

        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<PlayerSkillUpgradeRow> rows = await GetParsedTableData<PlayerSkillUpgradeRow>(ct);
            _repository = new TableRepository<PlayerSkillUpgradeRow>(rows);

            PlayerSkillUpgradeGroup group = null;
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                
                if (row.Skill_Requirement == 0)
                {
                    group = new PlayerSkillUpgradeGroup();
                    GroupRows.Add(group);
                }
                
                if(group is not null)
                    group.Rows.Add(row);
            }
        }

        public PlayerSkillUpgradeGroup GetPlayerSkillUpgradeGroup(int skillNo)
        {
            return GroupRows.Find(x => x.SkillNo.Equals(skillNo));
        }
        
        public PlayerSkillUpgradeRow GetPlayerSkillUpgradeFirstRowData(PlayerSkillUpgradeRow row)
        {
            return GroupRows.Where(x => x.Rows.Contains(row))
                .Select(x => x.Rows.First()).FirstOrDefault();
        }
    }
}