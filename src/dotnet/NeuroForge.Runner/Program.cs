Console.WriteLine("NeuroForge.Runner is starting...");

var factory = new NeuroForge.Factory.NeuroForgeFactory();
await factory.InitializeAsync();

Console.ReadLine();
