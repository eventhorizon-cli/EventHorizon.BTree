namespace EventHorizon.BTree;

internal enum InsertionBehavior
{
    /// <summary>
    /// 默认操作，如果 key 已经存在，则不会更新 value
    /// </summary>
    None = 0,

    /// <summary>
    /// 如果 key 已经存在，则更新 value
    /// </summary>
    OverwriteExisting = 1,

    /// <summary>
    /// 如果 key 已经存在，则抛出异常
    /// </summary>
    ThrowOnExisting = 2
}