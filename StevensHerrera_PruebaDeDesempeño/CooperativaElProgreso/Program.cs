// Composition root: wires up the in-memory store, repositories, services and menus,
// then drives the top-level loop between the teller window and the manager reports.

var store = new InMemoryStore();
SeedData.Apply(store); // Sample members/movements for local testing.

IMemberRepository memberRepository = new MemberRepository(store);
IMovementRepository movementRepository = new MovementRepository(store);
IExchangeRateService exchangeRateService = new TrmExchangeRateService();

IMemberService memberService = new MemberService(memberRepository, movementRepository, exchangeRateService);
IMovementService movementService = new MovementService(memberRepository, movementRepository);
IReportService reportService = new ReportService(memberRepository, movementRepository);

var cashierMenu = new CashierMenu(memberService, movementService);
var managerMenu = new ManagerMenu(reportService);

Console.WriteLine("Cooperativa Financiera El Progreso");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("===== Menú principal =====");
    Console.WriteLine("1. Ventanilla de cajero");
    Console.WriteLine("2. Reportes administrativos");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            await cashierMenu.RunAsync();
            break;
        case "2":
            managerMenu.Run();
            break;
        case "0":
            running = false;
            break;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

Console.WriteLine("Hasta pronto.");
