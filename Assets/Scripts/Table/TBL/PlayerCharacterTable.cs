using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

using Table.Models;

namespace Table.TBL
{
    public class PlayerCharacterTable : TableReader, ITableInterface<PlayerCharacterRow>
    {
        private TableRepository<PlayerCharacterRow> _repository = new (new List<PlayerCharacterRow>());
    
        public IReadOnlyList<PlayerCharacterRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out PlayerCharacterRow row) => _repository.TryGet(no, out row);

        public PlayerCharacterRow GetData(int no)
        {
            if (TryGet(no, out var row)) 
                return row;
            
            return null;
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<PlayerCharacterRow> rows = await GetParsedTableData<PlayerCharacterRow>(ct);
            _repository = new TableRepository<PlayerCharacterRow>(rows);
        }
    }
}