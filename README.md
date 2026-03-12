# SimpleReportGenerator

Minimal Blazor report table component with optional grouping.

## Install

```bash
dotnet add package SimpleReportGenerator
```

## Quick start

```razor
<ReportGenerator Data="items">
    <TableHeader>
        <tr>
            <th>Name</th>
            <th>Total</th>
        </tr>
    </TableHeader>
    <RowTemplate Context="row">
        <tr>
            <td>@row.Name</td>
            <td>@row.Total</td>
        </tr>
    </RowTemplate>
</ReportGenerator>
```

## Documentation

See [ReportGenerator.md](./ReportGenerator.md) for full usage, grouping, and parameter details.
