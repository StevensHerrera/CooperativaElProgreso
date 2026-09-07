# Cooperativa El Progreso

A console application for **Cooperativa Financiera El Progreso**, a savings cooperative with
around 300 members. It gives tellers a tool to register members, record deposits and
withdrawals, and gives the manager the reports needed to run the cooperative without
manually filtering a spreadsheet.

## Description

Members no longer touch the system directly: the teller looks a member up by document
number or name and operates on their behalf. Each member holds exactly one savings
account. The account balance is **never stored as a field** — it is always calculated
from the member's recorded movements, so it can't drift from reality or be edited by
hand.

Two movement types affect the balance differently:

- **Deposit**: adds the full amount to the balance.
- **Withdrawal**: subtracts the amount. Withdrawals over $1,000,000 COP also charge an
  $8,000 cash-handling fee. A withdrawal is rejected outright — not partially executed —
  if it (amount + fee) would leave the balance negative. Rejected movements are never
  persisted; the teller is told why.

The system also converts a member's balance to USD using the official TRM (Tasa
Representativa del Mercado), fetched asynchronously from Colombia's open-data portal,
and exposes six management reports from a dedicated menu.

## Architecture

The solution follows a **layered architecture** inside a single executable project:

```
CooperativaElProgreso/
├── Enums/            MovementType
├── Models/            Domain entities (Member, Movement) and value types
│   └── Reports/       Read-only DTOs for the six management reports
├── IInterfaces/        Contracts for repositories and services
├── Repositories/       In-memory persistence, implementing the repository contracts
├── Services/            Business rules (member management, movements, reporting)
├── Infrastructure/
│   └── ExternalServices/  TRM exchange-rate client (external API integration)
├── Data/               Shared in-memory store used by the repositories
└── UI/                 Console menus (teller window and manager reports)
```

The rationale for splitting `Infrastructure` from `Services`: fetching the exchange
rate is integration with a third party, not a business rule. Keeping it in its own
layer means a future change of provider only touches `TrmExchangeRateService`, and it
is the only class in the solution that depends on `HttpClient`.

`UI` is split into `CashierMenu` (the teller's day-to-day operations) and
`ManagerMenu` (the six reports), reflecting the two roles described in the
requirements.

`CooperativaElProgreso.Tests` is a separate xUnit project referencing the main
project, covering the core business rules (balance calculation, the withdrawal fee,
the never-negative rule, duplicate document rejection, and the report aggregations).

See `class-diagram.png` for the domain model with relationships and multiplicities.
An editable version of the class diagram is available in `diagrama-clases.mmd` and can be
opened with Mermaid Live Editor, GitHub or a Mermaid extension for VS Code.

## Technologies

- C# 14 / .NET 10 (console application)
- `System.Net.Http` for the asynchronous TRM lookup
- `System.Text.Json` for parsing the TRM API response

## Running the project

Requirements: .NET 10 SDK.

```bash
# Restore and run the console app
cd CooperativaElProgreso
dotnet run

# Run the unit tests
cd ../CooperativaElProgreso.Tests
dotnet test
```

Or open `CooperativaElProgreso.slnx` in Rider / Visual Studio and run the
`CooperativaElProgreso` project.

## Key technical decisions

- **Balance as a derived value, not a field.** `Member` has no `Balance` property;
  `MemberService.GetBalance` sums the `SignedTotal` of that member's movements on
  demand. This directly enforces the rule that the balance can never be corrected by
  hand.
- **`OperationResult<T>` instead of exceptions for business rejections.** Registering
  a duplicate document number, an invalid amount, or a withdrawal that would go
  negative are expected, everyday outcomes at a teller window — not exceptional
  program states. `OperationResult<T>` carries a success flag, an explanatory
  message, and the payload, so the UI layer can show a clear message without a
  try/catch.
- **The TRM client never throws.** `TrmExchangeRateService` catches network, timeout,
  and parsing failures internally and returns a failed `OperationResult` instead. The
  requirement is explicit that the application must keep operating normally if the
  external API is unavailable; letting an exception escape would violate that.
- **Movements are append-only.** `IMovementRepository` has no `Update` or `Delete`:
  once a movement is accepted it is permanent history, which matches "you can open
  the system and see everything that happened to a member since they joined."
- **Generic `ICrudOperations<T>`** is reused by `IMemberRepository` so member CRUD
  follows the same shape as the rest of the codebase, while `IMovementRepository` gets
  its own narrower contract (`Add`, not `Create`/`Update`/`Delete`) because movements
  behave differently from members.
- **Report DTOs live under `Models/Reports`,** separate from the domain entities,
  because they are read-only shapes assembled for a specific question ("who is
  dormant?", "how did we do this month?") rather than persisted domain concepts.
