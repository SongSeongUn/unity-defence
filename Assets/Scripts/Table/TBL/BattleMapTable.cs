using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;


namespace Table.TBL
{
    public class BattleMapTable : TableReader, ITableInterface<BattleMapRow>
    {
        private TableRepository<BattleMapRow> _repository = new (new List<BattleMapRow>());
    
        public IReadOnlyList<BattleMapRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out BattleMapRow row) => _repository.TryGet(no, out row);

        public BattleMapRow GetData(int no)
        {
            if (TryGet(no, out var row)) 
                return row;
            
            return null;
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<BattleMapRow> rows = await GetParsedTableData<BattleMapRow>(ct);
            _repository = new TableRepository<BattleMapRow>(rows);
        }
    }
}