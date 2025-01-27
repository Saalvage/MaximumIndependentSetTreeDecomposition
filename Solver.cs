
using System.Diagnostics;

namespace MaximumIndependentSetTreeDecomposition;

public class Solver {
    private readonly UniDirGraph _graph;
    private readonly IReadOnlyList<IReadOnlySet<int>> _bags;
    private readonly BiDirGraph _bagGraph;

    public Solver(UniDirGraph graph, IReadOnlyList<IReadOnlySet<int>> bags, BiDirGraph bagGraph) {
        _graph = graph;
        _bags = bags;
        _bagGraph = bagGraph;
    }

    public int Solve() => CalculateSolutions(0).Max(x => x.Weight);

    private IReadOnlyList<Solution> CalculateSolutions(int root) {
        var leaf = _bagGraph.ForwardGraph.FindLeaf(root);
        IReadOnlyList<Solution> solutions = [new(new HashSet<int>(), 0)];

        foreach (var node in _bags[leaf]) {
            solutions = solutions.IntroduceNode(_graph, node);
        }

        while (leaf != root) {
            var edges = _bagGraph.BackwardGraph.GetEdges(leaf);
            Debug.Assert(edges.Length == 1);
            var newNode = edges[0];
            solutions = Transition(leaf, newNode, solutions);
            var furtherEdges = _bagGraph.ForwardGraph.GetEdges(newNode);
            if (furtherEdges.Length != 1) {
                foreach (var other in furtherEdges) {
                    if (other != leaf) {
                        var otherSolutions = CalculateSolutions(other);
                        otherSolutions = Transition(other, newNode, otherSolutions);
                        solutions = solutions.Merge(otherSolutions);
                    }
                }
            }
            leaf = newNode;
        }

        return solutions;
    }

    private IReadOnlyList<Solution> Transition(int from, int to, IReadOnlyList<Solution> solutions) {
        var removed = _bags[from].Except(_bags[to]);
        foreach (var node in removed) {
            solutions = solutions.RemoveNode(node);
        }

        var added = _bags[to].Except(_bags[from]);
        foreach (var node in added) {
            solutions = solutions.IntroduceNode(_graph, node);
        }

        return solutions;
    }
}
