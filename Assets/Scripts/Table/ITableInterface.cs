using System.Collections.Generic;

namespace Table
{
    public interface ITableInterface<TRow>
    {
        IReadOnlyList<TRow> Rows { get; }
        bool TryGet(int no, out TRow row);
    }

    public interface ITableRow
    {
        int Key { get; }
    }
}