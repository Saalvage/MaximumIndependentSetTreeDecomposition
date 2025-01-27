
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

    private IReadOnlyList<Solution> CalculateSolutions(int root, int blockFrom = -1) {
        var edges = _bagGraph.NodeEdges[root];
        if (edges.Count == 1 && blockFrom != -1) {
            IReadOnlyList<Solution> solutions = [new(new HashSet<int>(), 0)];
            foreach (var node in _bags[root]) {
                solutions = solutions.IntroduceNode(_graph, node);
            }

            return solutions;
        }

        return edges.Where(x => x != blockFrom)
            .Select(x => (x, CalculateSolutions(x, root)))
            .Select(x => Transition(x.x, root, x.Item2))
            .Aggregate((x, y) => x.Merge(y))
            .ToArray();
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
