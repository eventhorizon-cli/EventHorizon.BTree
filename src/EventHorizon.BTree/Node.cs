using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EventHorizon.BTree;

[DebuggerDisplay("ItemsCount = {ItemsCount}, ChildrenCount = {ChildrenCount}, Height = {GetHeight()}")]
[DebuggerTypeProxy(typeof(Node<,>.DebugView))]
internal class Node<TKey, TValue>
{
    #region Fields

    private readonly IComparer<TKey> _comparer;
    private readonly int _degree;
    private readonly int _minItems;
    private readonly int _maxItems;
    private readonly int _maxChildren;

    private readonly Items<TKey, TValue?> _items;
    private readonly Children<TKey, TValue?> _children;

    #endregion

    #region Constructors

    public Node(int degree, IComparer<TKey> comparer)
    {
        _degree = degree;
        _comparer = comparer;
        _minItems = degree - 1;
        _maxItems = 2 * degree - 1;
        _maxChildren = 2 * degree;

        _items = new Items<TKey, TValue?>(_maxItems, _comparer);
        _children = new Children<TKey, TValue?>(_maxChildren);
    }

    #endregion

    #region Properties

    public int ItemsCount => _items.Count;

    public int ChildrenCount => _children.Count;

    public bool IsItemsFull => ItemsCount == _maxItems;
    public bool IsItemsEmpty => ItemsCount == 0;

    public bool IsLeaf => ChildrenCount == 0;

    #endregion

    #region Public Methods

    public void Add(Item<TKey, TValue?> item)
    {
        _items.InsertAt(ItemsCount, item);
    }

    public InsertionResult TryInsert(TKey key, TValue? value, InsertionBehavior behavior)
    {
        // 如果当前key已经存在, 根据插入行为决定是否替换
        if (_items.TryFindKey(key, out int index))
        {
            switch (behavior)
            {
                case InsertionBehavior.OverwriteExisting:
                    _items[index].Value = value;
                    return InsertionResult.Updated;
                case InsertionBehavior.ThrowOnExisting:
                    throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
                default:
                    return InsertionResult.None;
            }
        }

        // 如果当前节点是叶子节点，则直接插入
        if (IsLeaf)
        {
            // index 是新的 item 应该插入的位置，items 按顺序排列
            _items.InsertAt(index, new Item<TKey, TValue?>(key, value));
            return InsertionResult.Added;
        }

        // 如果当前节点的子节点已经满了，则需要分裂
        // 如果当前节点的子节点没有满，则不需要分裂
        // 如果当前节点的子节点分裂了，则需要判断当前key是否大于分裂后的中间key
        // 如果当前key大于分裂后的中间key，则需要向右边的子节点插入
        // 如果当前key小于分裂后的中间key，则需要向左边的子节点插入

        // index 是新的 item 应该插入的位置，如果当做children的索引，则代表应该插入的位置的右边的子节点
        if (MaybeSplitChildren(index))
        {
            // rightmostItem 是子节点分裂后的中间的 item，被提升到当前节点的 items 中的最后一个位置了
            var middleItemOfChild = _items[index];

            switch (_comparer.Compare(key, middleItemOfChild.Key))
            {
                case > 0:
                    // 如果当前key大于分裂后的中间key，则需要向右边的子节点插入
                    index++;
                    break;
                case < 0:
                    // 如果当前key小于分裂后的中间key，则需要向左边的子节点插入
                    break;
                default:
                    // 如果当前key等于分裂后的中间key，根据插入行为决定是否替换
                    switch (behavior)
                    {
                        case InsertionBehavior.OverwriteExisting:
                            middleItemOfChild.Value = value;
                            return InsertionResult.Updated;
                        case InsertionBehavior.ThrowOnExisting:
                            throw new ArgumentException(
                                $"An item with the same key has already been added. Key: {key}");
                        default:
                            return InsertionResult.None;
                    }
            }
        }

        // 往子节点插入
        return _children[index].TryInsert(key, value, behavior);
    }

    public bool TryFind(TKey key, out Item<TKey, TValue?> item)
    {
        if (_items.TryFindKey(key, out int index))
        {
            item = _items[index];
            return true;
        }

        if (IsLeaf)
        {
            item = default!;
            return false;
        }

        return _children[index].TryFind(key, out item);
    }

    public Node<TKey, TValue?> GetChild(int index)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return _children[index];
    }

    public void AddChild(Node<TKey, TValue?> child) => _children.InsertAt(ChildrenCount, child);

    public Item<TKey, TValue?> Max()
    {
        if (IsLeaf)
        {
            return _items[ItemsCount - 1];
        }

        return _children[ChildrenCount - 1].Max();
    }

    public Item<TKey, TValue?> Min()
    {
        if (IsLeaf)
        {
            return _items[0];
        }

        return _children[0].Min();
    }

    /// <summary>
    /// 将当前<see cref="Node{TKey,TValue}"/>分裂成两个<see cref="Node{TKey,TValue}"/>。
    /// </summary>
    /// <returns>中间位置的<see cref="Item{TKey,TValue}"/>和分裂后的第二个<see cref="Node{TKey,TValue}"/></returns>
    public (Item<TKey, TValue?> MiddleItem, Node<TKey, TValue?> SecnodNode) Split()
    {
        int middleIndex = ItemsCount / 2;
        var middleItem = _items[middleIndex];
        var secondNode = new Node<TKey, TValue?>(_degree, _comparer);

        // 将中间位置后的所有Item移动到新的Node中
        for (int i = middleIndex + 1; i < ItemsCount; i++)
        {
            secondNode._items.Add(_items[i]);
        }

        _items.Truncate(middleIndex);

        if (!IsLeaf)
        {
            // 将中间位置后的所有子节点移动到新的Node中
            for (int i = middleIndex + 1; i < ChildrenCount; i++)
            {
                secondNode._children.Add(_children[i]);
            }

            _children.Truncate(middleIndex + 1);
        }

        return (middleItem, secondNode);
    }

    public bool TryRemove(TKey? key, RemoveType removeType, [MaybeNullWhen(false)] out Item<TKey, TValue?> item)
    {
        int index = 0;
        bool found = false;
        if (removeType == RemoveType.Max)
        {
            if (IsLeaf)
            {
                if (_items.Count == 0)
                {
                    item = default;
                    return false;
                }

                // 如果是叶子节点，直接删除最后一个元素，就是删除最大的 Item
                item = _items.RemoveLast();
                return true;
            }

            // 当前节点不是叶子节点，需要找到最大的子节点，继续向下查找并删除
            index = ItemsCount;
        }

        if (removeType == RemoveType.Min)
        {
            if (IsLeaf)
            {
                if (_items.Count == 0)
                {
                    item = default;
                    return false;
                }

                // 当前节点是叶子节点，直接删除第一个元素，就是删除最小的 Item
                item = _items.RemoveAt(0);
                return true;
            }

            // 当前节点不是叶子节点，需要找到最小的子节点，继续向下查找并删除
            index = 0;
        }

        if (removeType == RemoveType.Item)
        {
            // 如果没有找到，index 表示的是 key 可能在的子树的索引
            found = _items.TryFindKey(key!, out index);

            if (IsLeaf)
            {
                // 如果是叶子节点，能找到就删除，找不到就返回 false，表示删除失败
                if (found)
                {
                    item = _items.RemoveAt(index);
                    return true;
                }

                item = default;
                return false;
            }
        }

        // 如果当前节点的左子节点的 Item 个数小于最小 Item 个数，就需要进行合并或者借元素
        // 这个处理对应两种情况：
        // 1. 要删除的 Item 不在当前节点的子节点中，为避免删除后导致数据所在节点的 Item 个数小于最小 Item 个数，需要先进行合并或者借元素。
        // 2. 要删除的 Item 就在当前节点中，为避免删除后导致当前节点的 Item 个数小于最小 Item 个数，需要先从左子节点中借一个 Item 过来，保证当前节点的 Item 数量不变。
        // 为此先要保证左子节点被借用后的 Item 个数不小于最小 Item 个数。
        if (_children[index].ItemsCount <= _minItems)
        {
            return GrowChildrenAndTryRemove(index, key!, removeType, out item);
        }

        var child = _children[index];

        if (found)
        {
            // 如果在当前节点找到了，就删除当前节点的 Item，然后将 左子节点 中的最大的 Item 移动到当前节点中
            // 以维持当前节点的 Item 个数不变，保证 B树 有 k 个子节点的非叶子节点拥有 k − 1 个键的特性。
            item = _items[index];
            child.TryRemove(default!, RemoveType.Max, out var stolenItem);
            _items[index] = stolenItem;
            return true;
        }

        return child.TryRemove(key!, removeType, out item);
    }


    public IEnumerable<Item<TKey, TValue?>> InOrderTraversal()
    {
        var itemsCount = ItemsCount;
        var childrenCount = ChildrenCount;
        if (IsLeaf)
        {
            for (int i = 0; i < itemsCount; i++)
            {
                yield return _items[i];
            }

            yield break;
        }

        // 左右子树并不是相当于当前的 node 而言，而是相对于每个 item 来说的
        for (int i = 0; i < itemsCount; i++)
        {
            if (i < childrenCount)
            {
                foreach (var item in _children[i].InOrderTraversal())
                {
                    yield return item;
                }
            }

            yield return _items[i];
        }

        // 最后一个 item 的右子树
        if (childrenCount > itemsCount)
        {
            foreach (var item in _children[childrenCount - 1].InOrderTraversal())
            {
                yield return item;
            }
        }
    }

    public int GetHeight() => IsLeaf ? 0 : _children[0].GetHeight() + 1;

    #endregion

    #region Private Methods

    /// <summary>
    /// 如果指定的子节点已满，则将其分裂为两个子节点，并将中间的 <see cref="Item{TKey,TValue}"/>> 插入到当前节点中。
    /// </summary>
    /// <param name="childIndex">指定的子节点的索引</param>
    /// <returns>True 表示已经分裂了子节点，False 表示没有分裂子节点</returns>
    private bool MaybeSplitChildren(int childIndex)
    {
        var childNode = _children[childIndex];
        if (childNode.IsItemsFull)
        {
            var (middleItem, secondNode) = childNode.Split();
            _items.InsertAt(childIndex, middleItem);
            // 将新node插入到当前node的children中
            _children.InsertAt(childIndex + 1, secondNode);
            return true;
        }

        return false;
    }

    private bool GrowChildrenAndTryRemove(
        int childIndex,
        TKey key,
        RemoveType removeType,
        [MaybeNullWhen(false)] out Item<TKey, TValue?> item)
    {
        if (childIndex > 0 && _children[childIndex - 1].ItemsCount > _minItems)
        {
            // 如果左边的子节点存在且左边的子节点的item数量大于最小值，则从左边的子节点借一个item
            var child = _children[childIndex];
            var leftChild = _children[childIndex - 1];
            var stolenItem = leftChild._items.RemoveLast();
            child._items.InsertAt(0, _items[childIndex - 1]);
            _items[childIndex - 1] = stolenItem;
            if (!leftChild.IsLeaf)
            {
                // 非叶子节点的子节点需要保证数量比item多1，item数量变了，子节点数量也要变
                // 所以需要从左边的子节点中移除最后一个子节点，然后插入到当前子节点的第一个位置
                child._children.InsertAt(0, leftChild._children.RemoveLast());
            }
        }
        else if (childIndex < ChildrenCount - 1 && _children[childIndex + 1].ItemsCount > _minItems)
        {
            // 如果右边的子节点存在且右边的子节点的item数量大于最小值，则从右边的子节点借一个item
            var child = _children[childIndex];
            var rightChild = _children[childIndex + 1];
            var stolenItem = rightChild._items.RemoveAt(0);
            child._items.Add(_items[childIndex]);
            _items[childIndex] = stolenItem;
            if (!rightChild.IsLeaf)
            {
                // 非叶子节点的子节点需要保证数量比item多1，item数量变了，子节点数量也要变
                // 所以需要从右边的子节点中移除第一个子节点，然后插入到当前子节点的最后一个位置
                child.AddChild(rightChild._children.RemoveAt(0));
            }
        }
        else
        {
            // 如果当前节点左右两边的子节点的item数量都不大于最小值（例如正好等于最小值 t-1 ），则合并当前节点和右边的子节点或者左边的子节点
            // 优先和右边的子节点合并，如果右边的子节点不存在，则和左边的子节点合并
            if (childIndex >= ItemsCount)
            {
                // ItemCount 代表最的子节点的索引，如果 childIndex 大于等于 ItemCount，说明右边的子节点不存在，需要和左边的子节点合并
                childIndex--;
            }

            var child = _children[childIndex];
            var mergeItem = _items.RemoveAt(childIndex);
            var mergeChild = _children.RemoveAt(childIndex + 1);
            child._items.Add(mergeItem);
            child._items.AddRange(mergeChild._items);
            child._children.AddRange(mergeChild._children);
        }

        return TryRemove(key, removeType, out item);
    }

    #endregion

    #region DebugView

    private class DebugView
    {
        private readonly Node<TKey, TValue> _node;

        public DebugView(Node<TKey, TValue> node)
        {
            _node = node;
        }

        public Items<TKey, TValue?> Items => _node._items;

        public Children<TKey, TValue?> Children => _node._children;
    }

    #endregion
}