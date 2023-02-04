namespace EventHorizon.BTree.Tests;

public class BTree_NonGeneric_Tests
{
    [Fact]
    public void BTree_Add_Remove_Random_Test()
    {
        // Assert
        var random = Random.Shared;
        var btree = new BTree<int, int>(random.Next(2, 32));

        var keysToAdd = new HashSet<int>();
        var keysToRemove = new HashSet<int>();

        for (int i = 0; i < 1000; i++)
        {
            var key = random.Next(0, 100000);
            keysToAdd.Add(key);
            btree.TryAdd(key, key);
        }

        for (int i = 0; i < 1000; i++)
        {
            var key = random.Next(0, 100000);
            keysToRemove.Add(key);
        }
        
        // Act
        foreach (var keyToRemove in keysToRemove)
        {
            var removed = btree.TryRemove(keyToRemove, out _);
            // Assert
            Assert.Equal(keysToAdd.Contains(keyToRemove), removed);
        }
    }

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
    public void BTree_Remove_From_Leaf()
    {
        var btree = new BTree<int, string>(3)
        {
            [1] = "1",
            [2] = "2",
            [3] = "3",
            [4] = "4",
            [5] = "5",
            [6] = "6",
            [7] = "7",
            [8] = "8",
            [9] = "9",
            [10] = "10",
            [11] = "11",
            [12] = "12"
        };

        btree.TryRemove(7, out var value);
        Assert.Equal("7", value);
        Assert.Equal(11, btree.Count);
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
        bool removed =  btree.TryRemoveMax(out var max);
        // Assert
        Assert.True(removed);
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
        bool removed = btree.TryRemoveMin(out var min);
        // Assert
        Assert.True(removed);
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
            Assert.Equal(btree.Count, 19 - i);
            Assert.Equal(btree.Count, btree.Count());
            Assert.Equal(i.ToString(), value);
        }

        Assert.Empty(btree);
    }
    
    [Fact]
    public void BTree_RemoveItem_From_Leaf()
    {
        // Arrange
        var btree = new BTree<int, string>(3);

        for (int i = 1; i <= 12; i++)
        {
            btree.Add(i, i.ToString());
        }

        // Act
        btree.TryRemove(11, out var value);

        // Assert
        Assert.Equal("11", value);
    }
    
    [Fact]
    public void BTree_RemoveItem_From_Non_Leaf()
    {
        // Arrange
        var btree = new BTree<int, string>(3);

        for (int i = 1; i <= 12; i++)
        {
            btree.Add(i, i.ToString());
        }

        // Act
        btree.TryRemove(3, out _);
        btree.TryRemove(6, out var value);

        // Assert
        Assert.Equal("6", value);
    }

    [Fact]
    public void BTree_RemoveItem_Steal_From_Left_Node()
    {
        // Arrange
        var btree = new BTree<int, string>(3);

        for (int i = 1; i <= 12; i++)
        {
            btree.Add(i, i.ToString());
        }

        // Act
        btree.TryRemove(3, out _);
        btree.TryRemove(7, out var value);

        // Assert
        Assert.Equal("7", value);
    }

    [Fact]
    public void BTree_RemoveItem_Steal_From_Right_Node()
    {
        // Arrange
        var btree = new BTree<int, string>(5);

        for (int i = 1; i <= 12; i++)
        {
            btree.Add(i, i.ToString());
        }

        // Act
        btree.TryRemove(4, out var value);

        // Assert
        Assert.Equal("4", value);
    }

    [Fact]
    public void BTree_RemoveItem_Merge_Node()
    {
        // Arrange
        var btree = new BTree<int, string>(4);

        for (int i = 1; i <= 12; i++)
        {
            btree.Add(i, i.ToString());
        }

        // Act
        btree.TryRemove(2, out var value);

        // Assert
        Assert.Equal("2", value);
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