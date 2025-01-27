using System.Diagnostics;

namespace MaximumIndependentSetTreeDecomposition;

// These classes are not general purpose, their methods are idiosyncratic.

// This graph representation is effectively like a tree.
public record UniDirGraph(IReadOnlySet<int>[] NodeEdges, bool IsBackwards = false) {
    public static async Task<UniDirGraph> CreateFromEdges(int nodeCount, IAsyncEnumerable<(int, int)> edges, bool reverse = false) {
        var nodeEdges = new HashSet<int>[nodeCount];
        Initialize();

        await foreach (var (from, to) in edges) {
            nodeEdges[from].Add(to);
        }

        return new(nodeEdges, reverse);

        void Initialize() {
            foreach (ref var node in nodeEdges.AsSpan()) {
                node = [];
            }
        }
    }

    public IReadOnlySet<int> GetEdges(int node) => NodeEdges[node];

    public bool HasEdge(int a, int b) {
        if (a > b ^ IsBackwards) {
            (a, b) = (b, a);
        }

        return GetEdges(a).Contains(b);
    }

    public int FindLeaf(int node) {
        while (true) {
            var edges = GetEdges(node);
            if (edges.Count == 0) {
                return node;
            }
            node = edges.First();
        }
    }

    public async Task<BiDirGraph> CreateBiDir() {
        return new(this, await UniDirGraph.CreateFromEdges(NodeEdges.Length, GetReverseEdges(), true));

        async IAsyncEnumerable<(int, int)> GetReverseEdges() {
            for (var i = 0; i < NodeEdges.Length; i++) {
                for (var j = 0; j < i; j++) {
                    if (HasEdge(j, i)) {
                        yield return (i, j);
                    }
                }
            }
        }

    }
}

// Maintaining both graphs like this gives us an implicitly rooted tree.
public record BiDirGraph(UniDirGraph ForwardGraph, UniDirGraph BackwardGraph) {
    public bool IsValid() {
        for (var i = 0; i < ForwardGraph.NodeEdges.Length; i++) {
            for (var j = 0; j < BackwardGraph.NodeEdges.Length; j++) {
                if (ForwardGraph.HasEdge(i, j) != BackwardGraph.HasEdge(i, j)) {
                    return false;
                }
            }
        }

        return true;
    }
}