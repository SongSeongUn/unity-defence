using System.Collections.Generic;

namespace Table
{
    public sealed class TableRepository<TRow> : ITableInterface<TRow> where TRow : ITableRow
    {
        private readonly List<TRow> _rows;
        private readonly Dictionary<int, TRow> _byNo;

        public IReadOnlyList<TRow> Rows => _rows;

        public TableRepository(IEnumerable<TRow> rows)
        {
            _rows = new List<TRow>();
            _byNo = new Dictionary<int, TRow>();

            if (rows == null)
                return;

            foreach (TRow row in rows)
            {
                if (row == null || row.Key == 0)
                    continue;

                _rows.Add(row);
                _byNo[row.Key] = row;
            }
        }

        /// <summary>
        /// 문자열 ID로 테이블 행을 찾습니다.
        /// </summary>
        public bool TryGet(int no, out TRow row)
        {
            return _byNo.TryGetValue(no, out row);
        }
    }
}