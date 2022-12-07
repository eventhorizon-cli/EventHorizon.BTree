using System.Diagnostics;

namespace EventHorizon.BTree;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(Items<,>.DebugView))]
internal class Items<TKey, TValue>
{
    #region Fields

    private readonly Item<TKey, TValue?>?[] _items;
    private readonly int _capacity;
    private readonly IComparer<TKey> _comparer;

    private int _count;

    #endregion

    #region Constructors

    public Items(int capacity, IComparer<TKey> comparer)
    {
        _capacity = capacity;
        _items = new Item<TKey, TValue?>[capacity];
        _comparer = comparer;
    }

    #region Properties

    public int Count => _count;

    #endregion

    #region Indexers

    public Item<TKey, TValue?> this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            return _items[index]!;
        }
        set => _items[index] = value;
    }

    #endregion

    #endregion

    #region Public Methods

    /// <summary>
    /// 查找指定的键，并返回它的索引，如果找不到则返回key可以插入的位置
    /// </summary>
    /// <param name="key">指定的key</param>
    /// <param name="index">key的索引或者其可以插入的位置</param>
    /// <returns>指定的key是否存在</returns>
    public bool TryFindKey(TKey key, out int index)
    {
        if (_count == 0)
        {
            index = 0;
            return false;
        }

        // 二分查找
        int left = 0;
        int right = _count - 1;
        while (left <= right)
        {
            int middle = (left + right) / 2;
            var compareResult = _comparer.Compare(key, _items[middle]!.Key);
            if (compareResult == 0)
            {
                index = middle;
                return true;
            }

            if (compareResult < 0)
            {
                right = middle - 1;
            }
            else
            {
                left = middle + 1;
            }
        }

        index = left;
        return false;
    }

    public void InsertAt(int index, Item<TKey, TValue?> item)
    {
        if (_count == _capacity)
            throw new InvalidOperationException("Cannot insert into a full list.");

        if (index < _count)
            Array.Copy(_items, index, _items, index + 1, _count - index);

        _items[index] = item;
        _count++;
    }

    public void Add(Item<TKey, TValue?> item) => InsertAt(_count, item);

    public void AddRange(Items<TKey, TValue?> items)
    {
        if (_count + items.Count > _capacity)
            throw new InvalidOperationException("Cannot add items to a full list.");

        Array.Copy(items._items, 0, _items, _count, items.Count);
        _count += items.Count;
    }

    public Item<TKey, TValue?> RemoveAt(int index)
    {
        if (index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var item = _items[index];

        if (index < _count - 1)
            Array.Copy(_items, index + 1, _items, index, _count - index - 1);

        _items[_count - 1] = null;
        _count--;

        return item!;
    }


    public Item<TKey, TValue?> RemoveLast() => RemoveAt(_count - 1);


    public void Truncate(int index)
    {
        if (index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        for (int i = index; i < _count; i++)
        {
            _items[i] = null;
        }

        _count = index;
    }

    #endregion

    #region DebugView

    private class DebugView
    {
        private readonly Items<TKey, TValue?> _items;

        public DebugView(Items<TKey, TValue?> items)
        {
            _items = items;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Item<TKey, TValue?>?[] Items => _items._items;
    }

    #endregion
}