using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;

namespace Table.TBL
{
    public class BattleExpTable : TableReader, ITableInterface<BattleExpRow>
    {
        private TableRepository<BattleExpRow> _repository = new (new List<BattleExpRow>());
    
        public IReadOnlyList<BattleExpRow> Rows => _repository.Rows;
        
        public bool TryGet(int level, out BattleExpRow row) => _repository.TryGet(level, out row);

        public BattleExpRow GetData(int level)
        {
            if (TryGet(level, out var row)) 
                return row;
            
            return null;
        }
        
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<BattleExpRow> rows = await GetParsedTableData<BattleExpRow>(ct);
            _repository = new TableRepository<BattleExpRow>(rows);
        }
    }
}