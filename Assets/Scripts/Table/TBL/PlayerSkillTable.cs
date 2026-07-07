using Table.Models;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Table.TBL
{
    public class PlayerSkillTable : TableReader, ITableInterface<PlayerSkillRow>
    {
        private TableRepository<PlayerSkillRow> _repository = new (new List<PlayerSkillRow>());
    
        public IReadOnlyList<PlayerSkillRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out PlayerSkillRow row) => _repository.TryGet(no, out row);
        
        public PlayerSkillRow GetData(int no)
        {
            if (TryGet(no, out var row)) 
                return row;
            
            return null;
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<PlayerSkillRow> rows = await GetParsedTableData<PlayerSkillRow>(ct);
            _repository = new TableRepository<PlayerSkillRow>(rows);
        }
    }
}