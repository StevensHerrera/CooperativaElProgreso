namespace CooperativaElProgreso.UI;

/// <summary>
/// Everything a teller does at the window: the member never touches the system,
/// so this menu covers registering, finding, updating and moving money for members.
/// </summary>
public class CashierMenu
{
    private readonly IMemberService _memberService;
    private readonly IMovementService _movementService;

    public CashierMenu(IMemberService memberService, IMovementService movementService)
    {
        _memberService = memberService;
        _movementService = movementService;
    }

    public async Task RunAsync()
    {
        var running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("===== Ventanilla de cajero =====");
            Console.WriteLine("1. Registrar nuevo asociado");
            Console.WriteLine("2. Listar todos los asociados");
            Console.WriteLine("3. Buscar asociado por número de documento");
            Console.WriteLine("4. Buscar asociado por nombre");
            Console.WriteLine("5. Actualizar datos del asociado");
            Console.WriteLine("6. Eliminar asociado");
            Console.WriteLine("7. Consultar saldo del asociado");
            Console.WriteLine("8. Consultar saldo en USD (TRM oficial)");
            Console.WriteLine("9. Registrar una consignación");
            Console.WriteLine("10. Registrar un retiro");
            Console.WriteLine("11. Ver todos los movimientos de un asociado");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RegisterMember(); break;
                case "2": ListMembers(); break;
                case "3": FindByDocumentNumber(); break;
                case "4": FindByName(); break;
                case "5": UpdateMember(); break;
                case "6": DeleteMember(); break;
                case "7": CheckBalance(); break;
                case "8": await CheckBalanceInUsdAsync(); break;
                case "9": RegisterDeposit(); break;
                case "10": RegisterWithdrawal(); break;
                case "11": ViewMovements(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    private void RegisterMember()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Nombre completo: ");
        var fullName = Console.ReadLine() ?? string.Empty;
        Console.Write("Número de teléfono (opcional): ");
        var phone = Console.ReadLine();
        Console.Write("Dirección (opcional): ");
        var address = Console.ReadLine();

        var result = _memberService.RegisterMember(documentNumber, fullName, phone, address);
        Console.WriteLine(result.Success ? $"Correcto: {result.Message}" : $"Rechazado: {result.Message}");
    }

    private void ListMembers()
    {
        var members = _memberService.ListMembers();
        if (members.Count == 0)
        {
            Console.WriteLine("Aún no hay asociados registrados.");
            return;
        }

        foreach (var m in members)
        {
            Console.WriteLine($"{m.DocumentNumber} | {m.FullName} | Saldo: COP $ {_memberService.GetBalance(m.Id):N0}");
        }
    }

    private void FindByDocumentNumber()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        var member = _memberService.FindByDocumentNumber(documentNumber);

        Console.WriteLine(member is null
            ? "No se encontró un asociado con ese número de documento."
            : $"{member.DocumentNumber} | {member.FullName} | {member.PhoneNumber} | {member.Address} | Saldo: COP $ {_memberService.GetBalance(member.Id):N0}");
    }

    private void FindByName()
    {
        Console.Write("Nombre (o parte del nombre): ");
        var namePart = Console.ReadLine() ?? string.Empty;
        var matches = _memberService.FindByName(namePart);

        if (matches.Count == 0)
        {
            Console.WriteLine("No hay asociados que coincidan con ese nombre.");
            return;
        }

        foreach (var m in matches)
        {
            Console.WriteLine($"{m.DocumentNumber} | {m.FullName}");
        }
    }

    private void UpdateMember()
    {
        Console.Write("Número de documento del asociado a actualizar: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Nuevo nombre completo (deje vacío para conservarlo): ");
        var fullName = Console.ReadLine();
        Console.Write("Nuevo teléfono (deje vacío para conservarlo): ");
        var phone = Console.ReadLine();
        Console.Write("Nueva dirección (deje vacío para conservarla): ");
        var address = Console.ReadLine();

        var result = _memberService.UpdateMember(
            documentNumber,
            string.IsNullOrWhiteSpace(fullName) ? null : fullName,
            string.IsNullOrWhiteSpace(phone) ? null : phone,
            string.IsNullOrWhiteSpace(address) ? null : address);

        Console.WriteLine(result.Success ? "Asociado actualizado." : $"Rechazado: {result.Message}");
    }

    private void DeleteMember()
    {
        Console.Write("Número de documento del asociado a eliminar: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        var result = _memberService.DeleteMember(documentNumber);
        Console.WriteLine(result.Success ? "Asociado eliminado." : $"Rechazado: {result.Message}");
    }

    private void CheckBalance()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        var member = _memberService.FindByDocumentNumber(documentNumber);

        Console.WriteLine(member is null
            ? "No se encontró un asociado con ese número de documento."
            : $"Saldo: COP $ {_memberService.GetBalance(member.Id):N0}");
    }

    private async Task CheckBalanceInUsdAsync()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        var result = await _memberService.GetBalanceInUsdAsync(documentNumber);

        if (!result.Success || result.Payload is null)
        {
            Console.WriteLine($"No se pudo convertir a USD: {result.Message}");
            return;
        }

        var payload = result.Payload;
        Console.WriteLine(
            $"Saldo: COP $ {payload.BalanceInCop:N0} = USD $ {payload.BalanceInUsd:N2} " +
            $"(TRM: COP $ {payload.RateUsed.Value:N2} por USD $ 1, vigente del {payload.RateUsed.ValidFrom:d} al {payload.RateUsed.ValidTo:d})");
    }

    private void RegisterDeposit()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Monto: ");
        var amount = ReadDecimal();

        var result = _movementService.RegisterDeposit(documentNumber, amount);
        Console.WriteLine(result.Success ? $"Correcto: {result.Message}" : $"Rechazado: {result.Message}");
    }

    private void RegisterWithdrawal()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Monto: ");
        var amount = ReadDecimal();

        var result = _movementService.RegisterWithdrawal(documentNumber, amount);
        Console.WriteLine(result.Success ? $"Correcto: {result.Message}" : $"Rechazado: {result.Message}");
    }

    private void ViewMovements()
    {
        Console.Write("Número de documento: ");
        var documentNumber = Console.ReadLine() ?? string.Empty;
        var movements = _movementService.GetMovementsForMember(documentNumber);

        if (movements.Count == 0)
        {
            Console.WriteLine("No se encontraron movimientos para este asociado.");
            return;
        }

        foreach (var m in movements.OrderBy(m => m.Date))
        {
            var type = m.Type == MovementType.Deposit ? "Consignación" : "Retiro";
            Console.WriteLine($"{m.Date:g} | {type} | COP $ {m.Amount:N0} | Comisión: COP $ {m.Fee:N0}");
        }
    }

    private static decimal ReadDecimal()
    {
        var input = Console.ReadLine();
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : -1;
    }
}
