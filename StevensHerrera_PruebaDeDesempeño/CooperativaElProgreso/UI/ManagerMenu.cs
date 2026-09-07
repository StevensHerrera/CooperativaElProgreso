namespace CooperativaElProgreso.UI;

/// <summary>The six reports Don Rafael asked for, available from a dedicated manager menu.</summary>
public class ManagerMenu
{
    private readonly IReportService _reportService;

    public ManagerMenu(IReportService reportService)
    {
        _reportService = reportService;
    }

    public void Run()
    {
        var running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("===== Reportes administrativos =====");
            Console.WriteLine("1. ¿Cuánto dinero tenemos?");
            Console.WriteLine("2. ¿Cuáles son nuestros mejores asociados?");
            Console.WriteLine("3. ¿Qué asociados están inactivos?");
            Console.WriteLine("4. ¿Cómo nos fue en un período?");
            Console.WriteLine("5. ¿Cuáles fueron los movimientos más grandes?");
            Console.WriteLine("6. ¿Qué asociados tienen mayor actividad?");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ShowSummary(); break;
                case "2": ShowTopMembers(); break;
                case "3": ShowDormantMembers(); break;
                case "4": ShowPeriodReport(); break;
                case "5": ShowTopMovements(); break;
                case "6": ShowCashierActivity(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    private void ShowSummary()
    {
        var report = _reportService.GetCooperativeSummary();
        Console.WriteLine($"Saldo total (COP): COP $ {report.TotalBalance:N0}");
        Console.WriteLine($"Cantidad de asociados: {report.MemberCount}");
        Console.WriteLine($"Saldo promedio (COP): COP $ {report.AverageBalance:N0}");
    }

    private void ShowTopMembers()
    {
        var report = _reportService.GetTopMembers();
        if (report.Count == 0)
        {
            Console.WriteLine("Aún no hay asociados registrados.");
            return;
        }

        foreach (var row in report)
        {
            Console.WriteLine($"{row.DocumentNumber} | {row.FullName} | Saldo: COP $ {row.Balance:N0}");
        }
    }

    private void ShowDormantMembers()
    {
        var report = _reportService.GetDormantMembers();
        if (report.Count == 0)
        {
            Console.WriteLine("No hay asociados inactivos.");
            return;
        }

        foreach (var row in report)
        {
            Console.WriteLine($"{row.DocumentNumber} | {row.FullName} | Registro: {row.RegisteredAt:d}");
        }
    }

    private void ShowPeriodReport()
    {
        Console.Write("Fecha inicial (yyyy-MM-dd): ");
        var start = ReadDate();
        Console.Write("Fecha final (yyyy-MM-dd): ");
        var end = ReadDate();

        if (start is null || end is null)
        {
            Console.WriteLine("La fecha ingresada no es válida.");
            return;
        }

        var report = _reportService.GetPeriodReport(start.Value, end.Value);
        Console.WriteLine($"Período: {report.StartDate:d} a {report.EndDate:d}");
        Console.WriteLine($"Total consignado (COP): COP $ {report.TotalDeposited:N0} ({report.DepositCount} movimientos)");
        Console.WriteLine($"Total retirado (COP): COP $ {report.TotalWithdrawn:N0} ({report.WithdrawalCount} movimientos)");
        Console.WriteLine($"Diferencia (COP): COP $ {report.Difference:N0}");
    }

    private void ShowTopMovements()
    {
        var report = _reportService.GetTopMovements();
        if (report.Count == 0)
        {
            Console.WriteLine("Aún no hay movimientos registrados.");
            return;
        }

        foreach (var row in report)
        {
            var type = row.Type == MovementType.Deposit ? "Consignación" : "Retiro";
            Console.WriteLine($"{row.Date:g} | {type} | COP $ {row.Amount:N0} | {row.MemberFullName}");
        }
    }

    private void ShowCashierActivity()
    {
        var report = _reportService.GetCashierActivity();
        if (report.Count == 0)
        {
            Console.WriteLine("Aún no hay asociados registrados.");
            return;
        }

        foreach (var row in report)
        {
            Console.WriteLine(
                $"{row.FullName} | Movimientos: {row.MovementCount} | Consignado: COP $ {row.TotalDeposited:N0} | " +
                $"Retirado: COP $ {row.TotalWithdrawn:N0} | Saldo: COP $ {row.CurrentBalance:N0}");
        }
    }

    private static DateTime? ReadDate()
    {
        var input = Console.ReadLine();
        return DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;
    }
}
