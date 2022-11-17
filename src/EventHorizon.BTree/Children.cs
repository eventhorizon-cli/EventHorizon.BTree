using System.Diagnostics;

namespace EventHorizon.BTree;

[DebuggerTypeProxy(typeof(Children<,>.DebugView))]
internal class Children<TKey, TValue>
{
    #region Fields

    private readonly Node<TKey, TValue?>?[] _children;
    private readonly int _capacity;

    private int _count;

    #endregion

    #region Constructors

    public Children(int capacity)
    {
        _capacity = capacity;
        _children = new Node<TKey, TValue?>[_capacity];
    }

    #endregion

    #region Properties

    public int Count => _count;

    #endregion

    #region Indexers

    public Node<TKey, TValue?> this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            return _children[index]!;
        }
    }

    #endregion

    #region Public Methods

    public void InsertAt(int index, Node<TKey, TValue?> child)
    {
        if (_count == _capacity)
            throw new InvalidOperationException("Cannot insert into a full list.");

        if (index < _count)
            Array.Copy(_children, index, _children, index + 1, _count - index);

        _children[index] = child;
        _count++;
    }


    public void Add(Node<TKey, TValue?> child) => InsertAt(_count, child);

    public void AddRange(Children<TKey, TValue?> children)
    {
        if (_count + children.Count > _capacity)
            throw new InvalidOperationException("Cannot add to a full list.");

        Array.Copy(children._children, 0, _children, _count, children.Count);
        _count += children.Count;
    }

    public Node<TKey, TValue?> RemoveAt(int index)
    {
        if (index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var child = _children[index];

        if (index < _count - 1)
            Array.Copy(_children, index + 1, _children, index, _count - index - 1);

        _children[_count - 1] = null;
        _count--;

        return child!;
    }

    public Node<TKey, TValue?> RemoveLast() => RemoveAt(_count - 1);

    public void Truncate(int index)
    {
        if (index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        for (var i = index; i < _count; i++)
            _children[i] = null;

        _count = index;
    }

    #endregion

    #region DebugView

    private class DebugView
    {
        private readonly Children<TKey, TValue?> _children;

        public DebugView(Children<TKey, TValue?> children)
        {
            _children = children;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Node<TKey, TValue?>?[] Items => _children._children;
    }

    #endregion
}