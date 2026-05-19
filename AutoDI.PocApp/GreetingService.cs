namespace AutoDI.PocApp
{
    [RegisterScoped]
    public class GreetingService : IGreetingService
    {
        public string Greet(string name) => $"Hello, {name}!";
    }
}
