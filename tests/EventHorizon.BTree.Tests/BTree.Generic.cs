namespace EventHorizon.BTree.Tests;

public class BTree_Generic_Tests_string_string : BTree_Generic_Tests<string, string>
{
    protected override string CreateTKey(int seed)
    {
        int stringLength = seed % 10 + 5;
        Random rand = new Random(seed);
        byte[] bytes1 = new byte[stringLength];
        rand.NextBytes(bytes1);
        return Convert.ToBase64String(bytes1);
    }

    protected override string CreateTValue(int seed) => CreateTKey(seed);
}

public class BTree_Generic_Tests_int_int : BTree_Generic_Tests<int, int>
{
    protected override int CreateTKey(int seed)
    {
        Random rand = new Random(seed);
        return rand.Next();
    }

    protected override int CreateTValue(int seed) => CreateTKey(seed);
}