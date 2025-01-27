using MaximumIndependentSetTreeDecomposition;

var path = args.ElementAtOrDefault(0) ?? ReadInput();

string ReadInput() {
    Console.Write("Please enter the graph's path: ");
    return Console.ReadLine()!;
}

if (Directory.Exists(path)) {
    foreach (var file in Directory.EnumerateFiles(path, "*.gr")) {
        await Solve(file);
    }
} else {
    await Solve(path);
}

static async Task Solve(string file) {
    var (graph, bags, bagGraph) = await Input.Load(file);
    var start = DateTimeOffset.UtcNow;
    var solver = new Solver(graph, bags, bagGraph);
    var solution = solver.Solve();
    var time = DateTimeOffset.UtcNow - start;
    Console.WriteLine($"Calculated graph {Path.GetFileName(file)}'s maximum independent vertex set size to be {solution} in {time.TotalMilliseconds}ms");
}
