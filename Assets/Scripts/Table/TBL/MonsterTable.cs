using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;

namespace Table.TBL
{
    public class MonsterTable : TableReader, ITableInterface<MonsterRow>
    {
        private TableRepository<MonsterRow> _repository = new (new List<MonsterRow>());
    
        public IReadOnlyList<MonsterRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out MonsterRow row) => _repository.TryGet(no, out row);
        
        public MonsterRow GetData(int no)
        {
            if (TryGet(no, out var row)) 
                return row;
            
            return null;
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)

        {
            List<MonsterRow> rows = await GetParsedTableData<MonsterRow>(ct);
            _repository = new TableRepository<MonsterRow>(rows);
        }
    }
}