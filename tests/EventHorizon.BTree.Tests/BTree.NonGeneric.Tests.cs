namespace EventHorizon.BTree.Tests;

public class BTree_NonGeneric_Tests
{
    [Fact]
    public void BTree_Add_Key_With_Customer_Comparer()
    {
        var btree = new BTree<Foo, string>(3, new FooComparer());
        for (int i = 0; i < 10; i++)
        {
            btree.Add(new Foo(i), i.ToString());
        }

        Assert.Equal(10, btree.Count);
    }

    [Fact]
    public void BTree_Indexer_Key_With_Customer_Comparer()
    {
        var btree = new BTree<Foo, string>(3, new FooComparer());
        for (int i = 0; i < 10; i++)
        {
            btree[new Foo(i)] = i.ToString();
        }

        btree[new Foo(5)] = "55";
        Assert.Equal(10, btree.Count);
        Assert.Equal("55", btree[new Foo(5)]);
    }

    [Fact]
    public void BTree_Max_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        var max = btree.Max();
        // Assert
        Assert.Equal(17, max.Key.Bar);
        Assert.Equal("17", max.Value);
    }

    [Fact]
    public void BTree_Min_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        var min = btree.Min();
        // Assert
        Assert.Equal(1, min.Key.Bar);
        Assert.Equal("1", min.Value);
    }

    [Fact]
    public void BTree_Enumerate_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        int index = 1;
        foreach (var item in btree)
        {
            // Assert
            Assert.Equal(index++, item.Key.Bar);
        }
    }

    [Fact]
    public void BTree_Enumerate_Reverse_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        int index = 17;
        foreach (var item in btree.Reverse())
        {
            // Assert
            Assert.Equal(index--, item.Key.Bar);
        }
    }

    [Fact]
    public void BTree_RemoveMax_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        btree.TryRemoveMax(out var max);
        // Assert
        Assert.Equal("17", max);
    }

    [Fact]
    public void BTree_RemoveMin_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        btree.TryRemoveMin(out var min);
        // Assert
        Assert.Equal("1", min);
    }

    [Fact]
    public void BTree_RemoveItem_With_Customer_Comparer()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(19)] = "19",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
            [new Foo(18)] = "18",
            [new Foo(10)] = "10",

        };
        // Act
        for (int i = 1; i <= 19; i++)
        {
            btree.TryRemove(new Foo(i), out var value);
            // Assert
            Assert.Equal(i.ToString(), value);
        }

        Assert.Empty(btree);
        Assert.Equal(0, btree.Count);
    }

    [Fact]
    public void BTree_TryGetValue()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        for (int i = 1; i <= 17; i++)
        {
            btree.TryGetValue(new Foo(i), out var value);
            // Assert
            Assert.Equal(i.ToString(), value);
        }
    }

    [Fact]
    public void BTree_Contains_Key()
    {
        // Arrange
        var btree = new BTree<Foo, string>(5, new FooComparer())
        {
            [new Foo(10)] = "10",
            [new Foo(11)] = "11",
            [new Foo(12)] = "12",
            [new Foo(4)] = "4",
            [new Foo(5)] = "5",
            [new Foo(13)] = "13",
            [new Foo(8)] = "8",
            [new Foo(16)] = "16",
            [new Foo(17)] = "17",
            [new Foo(1)] = "1",
            [new Foo(14)] = "14",
            [new Foo(15)] = "15",
            [new Foo(6)] = "6",
            [new Foo(7)] = "7",
            [new Foo(2)] = "2",
            [new Foo(3)] = "3",
            [new Foo(9)] = "9",
        };
        // Act
        for (int i = 1; i <= 17; i++)
        {
            bool contains = btree.ContainsKey(new Foo(i));
            // Assert
            Assert.True(contains);
        }
    }
}

public class Foo : IComparable<Foo>
{
    public Foo(int bar) => Bar = bar;
    public int Bar { get; set; }

    public int CompareTo(Foo? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Bar.CompareTo(other.Bar);
    }
}

public class FooComparer : IComparer<Foo>
{
    public int Compare(Foo x, Foo y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return x.Bar.CompareTo(y.Bar);
    }
}