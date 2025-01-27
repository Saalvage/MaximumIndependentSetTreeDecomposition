namespace MaximumIndependentSetTreeDecomposition;

// These classes are not general purpose, they are somewhat idiosyncratic.

// This graph representation is effectively like a tree.
public record UniDirGraph(IReadOnlySet<int>[] NodeEdges) {
    public static async Task<UniDirGraph> CreateFromEdges(int nodeCount, IAsyncEnumerable<(int, int)> edges) {
        var nodeEdges = new HashSet<int>[nodeCount];
        Initialize();

        await foreach (var (from, to) in edges) {
            nodeEdges[from].Add(to);
        }

        return new(nodeEdges);

        void Initialize() {
            foreach (ref var node in nodeEdges.AsSpan()) {
                node = [];
            }
        }
    }

    public bool HasEdge(int a, int b) {
        if (a > b) {
            (a, b) = (b, a);
        }

        return NodeEdges[a].Contains(b);
    }

    public BiDirGraph CreateBiDir() {
        var newNodeEdges = new HashSet<int>[NodeEdges.Length];
        for (var i = 0; i < NodeEdges.Length; i++) {
            newNodeEdges[i] = new(NodeEdges[i]);
        }

        for (var i = 0; i < NodeEdges.Length; i++) {
            foreach (var j in NodeEdges[i]) {
                newNodeEdges[j].Add(i);
            }
        }

        return new(newNodeEdges);
    }
}

public record BiDirGraph(IReadOnlySet<int>[] NodeEdges) {
    public bool IsValid() {
        for (var i = 0; i < NodeEdges.Length; i++) {
            if (NodeEdges[i].Any(j => !NodeEdges[j].Contains(i))) {
                return false;
            }
        }

        return true;
    }
}