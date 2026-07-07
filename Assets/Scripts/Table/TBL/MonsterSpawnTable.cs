using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;


namespace Table.TBL
{
    public class MonsterSpawnTable : TableReader, ITableInterface<MonsterSpawnRow>
    {
        private TableRepository<MonsterSpawnRow> _repository = new (new List<MonsterSpawnRow>());
    
        public IReadOnlyList<MonsterSpawnRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out MonsterSpawnRow row) => _repository.TryGet(no, out row);
        
        public MonsterSpawnRow GetData(int no)
        {
            if (TryGet(no, out var row)) 
                return row;
            
            return null;
        }

        public List<MonsterSpawnRow> GetStageData(int stageNo)
        {
            return Rows.Where(x => x.Map_No == stageNo).ToList();
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<MonsterSpawnRow> rows = await GetParsedTableData<MonsterSpawnRow>(ct);
            _repository = new TableRepository<MonsterSpawnRow>(rows);
        }
    }
}