using System.Diagnostics;

namespace MaximumIndependentSetTreeDecomposition;

// These classes are not general purpose, their methods are idiosyncratic.

// This graph representation is effectively like a tree.
public record UniDirGraph(int[] Nodes, int[] Edges, bool IsBackwards = false) {
    public static async Task<UniDirGraph> CreateFromEdges(int nodeCount, int maxEdgeCount, IAsyncEnumerable<(int, int)> orderedEdges, bool reverse = false) {
        var nodes = new int[nodeCount + 1];
        // Having space for more edges than necessary is no problem because they are simply not referenced.
        var edges = new int[maxEdgeCount];
        var prevNode = 0;
        await foreach (var (from, to) in orderedEdges) {
            Debug.Assert(from < to ^ reverse, $"Edge is not ordered correctly {from}, {to}!");
            if (prevNode != from) {
                Debug.Assert(prevNode < from, $"Nodes are not ordered correctly ({from} after {prevNode})!");

                MoveTo(from);
                prevNode = from;
            }
            edges[nodes[from + 1]++] = to;
        }
        MoveTo(nodes.Length - 2);

        return new(nodes, edges, reverse);

        void MoveTo(int end) {
            for (var i = prevNode + 1; i <= end; i++) {
                nodes[i + 1] = nodes[i];
            }
        }
    }

    public ReadOnlySpan<int> GetEdges(int node) => Edges.AsSpan()[Nodes[node]..Nodes[node + 1]];

    public bool HasEdge(int a, int b) {
        if (a > b ^ IsBackwards) {
            (a, b) = (b, a);
        }

        return GetEdges(a).Contains(b);
    }

    public int FindLeaf(int node) {
        while (true) {
            var edges = GetEdges(node);
            if (edges.Length == 0) {
                Debug.Assert(IsLeaf(node));
                return node;
            }
            node = edges[0];
        }
    }

    private bool IsLeaf(int node) => Enumerable.Range(0, Nodes.Length - 1).Count(x => HasEdge(node, x)) == 1;

    public async Task<BiDirGraph> CreateBiDir() {
        return new(this, await UniDirGraph.CreateFromEdges(Nodes.Length - 1, Nodes[^1], GetReverseEdges(), true));

        async IAsyncEnumerable<(int, int)> GetReverseEdges() {
            for (var i = 0; i < Nodes.Length - 1; i++) {
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
        for (var i = 0; i < ForwardGraph.Nodes.Length - 1; i++) {
            for (var j = 0; j < BackwardGraph.Nodes.Length - 1; j++) {
                if (ForwardGraph.HasEdge(i, j) != BackwardGraph.HasEdge(i, j)) {
                    return false;
                }
            }
        }

        return true;
    }
}