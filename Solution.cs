﻿using System.Collections.Frozen;

namespace MaximumIndependentSetTreeDecomposition;

public readonly record struct Solution(IReadOnlySet<int> Nodes, int Weight);

public static class SolutionExtensions {
    public static IReadOnlyList<Solution> IntroduceNode(this IReadOnlyList<Solution> previousSolutions, UniDirGraph graph, int node)
        => previousSolutions.Concat(previousSolutions
                .Where(x => !x.Nodes.Any(n => graph.HasEdge(n, node)))
                .Select(x => new Solution(x.Nodes.Append(node).ToFrozenSet(), x.Weight + 1)))
            .ToArray();

    public static IReadOnlyList<Solution> RemoveNode(this IReadOnlyList<Solution> previousSolutions, int node)
        => previousSolutions
            // Will be inspected later.
            .Where(x => !x.Nodes.Contains(node))
            .Select(x => x with {
                // Compare with the potential other solution which contained the node.
                Weight = Math.Max(x.Weight, previousSolutions.FirstOrDefault(y => y.Nodes.SetEquals(x.Nodes.Append(node))).Weight),
            }).ToArray();

    public static IReadOnlyList<Solution> Merge(this IReadOnlyList<Solution> leftSolutions, IReadOnlyList<Solution> rightSolutions)
        => leftSolutions.Select(x => x with {
                Weight = x.Weight + rightSolutions.First(y => y.Nodes.SetEquals(x.Nodes)).Weight - x.Nodes.Count,
            })
            .ToArray();
}
