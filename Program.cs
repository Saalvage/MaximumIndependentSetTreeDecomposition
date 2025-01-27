using MaximumIndependentSetTreeDecomposition;

var path = args.ElementAtOrDefault(0) ?? ReadInput();

string ReadInput() {
    Console.Write("Please enter the graph's path: ");
    return Console.ReadLine()!;
}

const int ITERATIONS = 100;
var dic = new Dictionary<string, TimeSpan>();

if (Directory.Exists(path)) {
    for (var i = 0; i < ITERATIONS; i++) {
        foreach (var file in Directory.EnumerateFiles(path, "*.gr")) {
            await Solve(file);
        }
    }
} else {
    await Solve(path);
}

foreach (var (file, time) in dic) {
    Console.WriteLine($"Average time for {Path.GetFileName(file)} was {time.TotalMilliseconds / ITERATIONS}ms");
}

async Task Solve(string file) {
    var (graph, bags, bagGraph) = await Input.Load(file);
    var start = DateTimeOffset.UtcNow;
    var solver = new Solver(graph, bags, bagGraph);
    var solution = solver.Solve();
    var time = DateTimeOffset.UtcNow - start;
    Console.WriteLine($"Calculated graph {Path.GetFileName(file)}'s maximum independent vertex set size to be {solution} in {time.TotalMilliseconds}ms");
    if (dic.ContainsKey(file)) {
        dic[file] += time;
    } else {
        dic.Add(file, time);
    }
}
