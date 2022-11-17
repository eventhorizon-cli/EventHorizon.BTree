namespace EventHorizon.BTree.Tests;

public abstract class BTree_Generic_Tests<TKey, TValue>

{
    #region BTree<Tkey, TValue?> Helper Methods

    protected BTree<TKey, TValue?> GenericBTreeFactory(int degree) => new(degree);

    protected BTree<TKey, TValue?> GenericBTreeFactory(int degree, IComparer<TKey> comparer) => new(degree, comparer);

    protected abstract TKey CreateTKey(int seed);

    protected abstract TValue? CreateTValue(int seed);

    #endregion

    [Fact]
    public void BTree_Add_ThrowsException_WhenKeyAlreadyExists()
    {
        var btree = GenericBTreeFactory(3);
        var key = CreateTKey(1);
        var value = CreateTValue(1);
        btree.Add(key, value);
        Assert.Throws<ArgumentException>(() => btree.Add(key, value));
    }

    [Fact]
    public void BTree_AddOrUpdate_Key_Not_Exists()
    {
        // Arrange
        var btree = GenericBTreeFactory(3);
        var key = CreateTKey(1);
        var value = CreateTValue(1);
        // Act
        btree[key] = value;
        // Assert
        Assert.Equal(1, btree.Count);
        Assert.Equal(value, btree[key]);
    }

    [Fact]
    public void BTree_AddOrUpdate_Key_Exists()
    {
        // Arrange
        var btree = GenericBTreeFactory(3);
        var key = CreateTKey(1);
        var value = CreateTValue(1);
        var newValue = CreateTValue(2);
        // Act
        btree[key] = value;
        btree[key] = newValue;
        // Assert
        Assert.Equal(1, btree.Count);
        Assert.Equal(newValue, btree[key]);
    }

    [Fact]
    public void BTree_Generic_Constructor_Throws_ArgumentOutOfRangeException_When_Degree_Is_Less_Than_2()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GenericBTreeFactory(1));
    }

    [Fact]
    public void BTree_Generic_Constructor_Throws_ArgumentNullException_When_Comparer_Is_Null()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() => GenericBTreeFactory(3, null!));
    }
}