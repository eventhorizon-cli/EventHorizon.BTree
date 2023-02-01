using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EventHorizon.BTree;

[DebuggerDisplay("Degree = {Degree}, Count = {Count}, Height = {GetHeight()}")]
[DebuggerTypeProxy(typeof(BTree<,>.DebugView))]
public sealed class BTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue?>>
{
    #region Fields

    private readonly int _degree;
    private readonly IComparer<TKey> _comparer;
    private int _count;
    private Node<TKey, TValue?>? _root;

    #endregion

    #region Constructors

    public BTree(int degree) : this(degree, Comparer<TKey>.Default)
    {
    }

    public BTree(int degree, IComparer<TKey> comparer)
    {
        if (degree < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(degree), "Degree must be at least 2.");
        }

        ArgumentNullException.ThrowIfNull(comparer);

        _degree = degree;
        _comparer = comparer;
    }

    #endregion

    #region Properties

    public int Count => _count;

    public int Degree => _degree;

    public IComparer<TKey> Comparer => _comparer;

    #endregion

    #region Indexers

    public TValue? this[[NotNull] TKey key]
    {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }
        set => TryInsert(key, value, InsertionBehavior.OverwriteExisting);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 往B树中添加一个键值对
    /// </summary>
    /// <param name="key">要添加的元素的key</param>
    /// <param name="value">要添加的元素的value</param>
    /// <exception cref="ArgumentNullException">key是null</exception>
    /// <exception cref="ArgumentException">key已经存在</exception>
    public void Add([NotNull] TKey key, TValue? value) =>
        TryInsert(key, value, InsertionBehavior.ThrowOnExisting);

    /// <summary>
    /// 尝试往B树中添加一个键值对
    /// </summary>
    /// <param name="key">要添加的元素的key</param>
    /// <param name="value">要添加的元素的value</param>
    /// <returns>true:添加成功;false:添加失败</returns>
    public bool TryAdd([NotNull] TKey key, TValue? value) =>
        TryInsert(key, value, InsertionBehavior.None);

    public bool ContainsKey([NotNull] TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return _root?.TryFind(key, out _) ?? false;
    }

    public bool TryGetValue([NotNull] TKey key, out TValue? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_root == null)
        {
            value = default;
            return false;
        }

        if (!_root.TryFind(key, out var item))
        {
            value = default;
            return false;
        }

        value = item.Value;
        return true;
    }

    public KeyValuePair<TKey, TValue?> Max()
    {
        if (_root == null)
        {
            throw new InvalidOperationException("BTree is empty.");
        }

        var maxItem = _root.Max();
        return new KeyValuePair<TKey, TValue?>(maxItem.Key, maxItem.Value);
    }

    public KeyValuePair<TKey, TValue?> Min()
    {
        if (_root == null)
        {
            throw new InvalidOperationException("BTree is empty.");
        }

        var minItem = _root.Min();
        return new KeyValuePair<TKey, TValue?>(minItem.Key, minItem.Value);
    }

    public bool TryRemove([NotNull] TKey key, out TValue? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        return TryRemove(key, RemoveType.Item, out value);
    }

    public bool TryRemoveMax(out TValue? value) => TryRemove(default, RemoveType.Max, out value);

    public bool TryRemoveMin(out TValue? value) => TryRemove(default, RemoveType.Min, out value);

    public IEnumerator<KeyValuePair<TKey, TValue?>> GetEnumerator()
    {
        foreach (var item in _root!.InOrderTraversal())
        {
            yield return new KeyValuePair<TKey, TValue?>(item.Key, item.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int GetHeight()
    {
        return _root?.GetHeight() ?? 0;
    }

    #endregion

    #region Private Methods

    private bool TryInsert([NotNull] TKey key, TValue? value, InsertionBehavior behavior)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_root == null)
        {
            _root = new Node<TKey, TValue?>(_degree, _comparer);
            _root.Add(new Item<TKey, TValue?>(key, value));
            _count++;
            return true;
        }

        if (_root.IsItemsFull)
        {
            // 根节点已满，需要分裂
            var (middleItem, secondNode) = _root.Split();
            var oldRoot = _root;
            _root = new Node<TKey, TValue?>(_degree, _comparer);
            // 将原来根节点中间的元素添加到新的根节点
            _root.Add(middleItem);
            // 将原来根节点分裂出来的节点添加到新的根节点
            _root.AddChild(oldRoot);
            _root.AddChild(secondNode);
        }

        // 从根节点开始插入，如果插入的 Key 已经存在，会按照 behavior 的值进行处理
        var insertionResult = _root.TryInsert(key, value, behavior);
        if (insertionResult == InsertionResult.Added) _count++;

        return insertionResult != InsertionResult.None;
    }

    private bool TryRemove(TKey? key, RemoveType removeType, out TValue? value)
    {
        if (_root == null || _root.IsItemsEmpty)
        {
            value = default;
            return false;
        }

        bool removed = _root.TryRemove(key, removeType, out var item);
        if (_root.IsItemsEmpty && !_root.IsLeaf)
        {
            // 根节点原来的两个子节点进行了合并，根节点唯一的元素被移动到了子节点中，需要将合并后的子节点设置为新的根节点
            _root = _root.GetChild(0);
        }

        if (removed)
        {
            _count--;
            value = item!.Value;
            return true;
        }

        value = default;
        return removed;
    }

    #endregion

    #region DebugView

    private class DebugView
    {
        private readonly BTree<TKey, TValue> _btree;

        public DebugView(BTree<TKey, TValue> bTree)
        {
            _btree = bTree;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue?>[] Items => _btree.ToArray();
    }

    #endregion
}