using System;

var environment = new Campus
{
    CoffeeMachineWorking = false,
    CafeOpen = true
};

var agent = new CoffeeAgent(environment);

agent.Run();

class Campus
{
    public bool CoffeeMachineWorking { get; set; }
    public bool CafeOpen { get; set; }
}

class CoffeeAgent(Campus campus)
{
    public void Run()
    {
        // 1. Obserwacja
        Console.WriteLine("Sprawdzam automat...");

        // 2. Decyzja i działanie
        if (campus.CoffeeMachineWorking)
        {
            Console.WriteLine("Kupuję kawę z automatu.");
        }
        else if (campus.CafeOpen)
        {
            Console.WriteLine("Automat nie działa. Idę do kawiarni.");
        }
        else
        {
            Console.WriteLine("Nie mogę kupić kawy. Pytam studenta, co dalej.");
        }
    }
}
