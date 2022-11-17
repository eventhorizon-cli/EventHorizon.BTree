namespace EventHorizon.BTree;

internal class Item<TKey, TValue>

{
    #region Constructors

    public Item(TKey key, TValue? value)
    {
        Key = key;
        Value = value;
    }

    #endregion

    #region Properties

    public TKey Key { get; }

    public TValue? Value { get; set; }
    
    #endregion

}