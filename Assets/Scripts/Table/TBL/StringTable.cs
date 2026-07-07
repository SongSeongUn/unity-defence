using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Table.Models;

namespace Table.TBL
{
    public class StringTable : TableReader, ITableInterface<StringRow>
    {
        private TableRepository<StringRow> _repository = new (new List<StringRow>());
    
        public IReadOnlyList<StringRow> Rows => _repository.Rows;
        
        public bool TryGet(int no, out StringRow row) => _repository.TryGet(no, out row);

        public string GetString(int no)
        {
            if (TryGet(no, out var row))
                return row.Kr;

            return null;
        }
        
        public override async UniTask InitializeAsync(CancellationToken ct)
        {
            List<StringRow> rows = await GetParsedTableData<StringRow>(ct);
            _repository = new TableRepository<StringRow>(rows);
        }
    }
}