using System.Diagnostics;

namespace MaximumIndependentSetTreeDecomposition;

public static class Input {
    public static async Task<(UniDirGraph Graph, IReadOnlyList<IReadOnlySet<int>> Bags, BiDirGraph BagGraph)> Load(string path) {
        path = Path.ChangeExtension(path, null);
        var graph = await LoadGraph($"{path}.gr");
        var (bags, bagGraph) = await LoadDecomposition($"{path}.td");
        Debug.Assert(bagGraph.IsValid());
        return (graph, bags, bagGraph);
    }

    private static async Task<UniDirGraph> LoadGraph(string file) {
        await using var stream = File.OpenRead(file);
        using var reader = new StreamReader(stream);
        var info = (await reader.ReadLineAsync())!.Split(' ');
        return await UniDirGraph.CreateFromEdges(int.Parse(info[2]), ReadEdges(reader));
    }

    private static async Task<(IReadOnlySet<int>[] Bags, BiDirGraph BagGraph)> LoadDecomposition(string file) {
        await using var stream = File.OpenRead(file);
        using var reader = new StreamReader(stream);
        var info = (await reader.ReadLineAsync())!.Split(' ');

        var bags = new IReadOnlySet<int>[int.Parse(info[2])];

        for (var i = 0; i < bags.Length; i++) {
            var bag = await reader.ReadLineAsync();
            Debug.Assert(bag![0] == 'b');
            var split = bag.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bags[int.Parse(split[1]) - 1] = split.Skip(2).Select(int.Parse).Select(x => x - 1).ToHashSet();
        }

        return (bags, (await UniDirGraph.CreateFromEdges(bags.Length, ReadEdges(reader))).CreateBiDir());
    }

    private static async IAsyncEnumerable<(int, int)> ReadEdges(TextReader reader) {
        while (await reader.ReadLineAsync() is { } line) {
            var split = line.Split(' ');
            var from = int.Parse(split[0]);
            var to = int.Parse(split[1]);
            yield return (from - 1, to - 1);
        }
    }
}
