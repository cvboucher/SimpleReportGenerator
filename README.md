# SimpleReportGenerator

Minimal Blazor report table component with templated headers and rows.

## Install

```bash
dotnet add package SimpleReportGenerator
```

## Usage

```razor
<ReportGenerator Items="items">
    <HeaderTemplate>
        <tr>
            <th>Name</th>
            <th>Total</th>
        </tr>
    </HeaderTemplate>
    <RowTemplate Context="row">
        <tr>
            <td>@row.Name</td>
            <td>@row.Total</td>
        </tr>
    </RowTemplate>
</ReportGenerator>
```

## Props

- `Items`: `IEnumerable<TItem>`
- `HeaderTemplate`: optional `RenderFragment`
- `RowTemplate`: required `RenderFragment<TItem>`
- `EmptyTemplate`: optional `RenderFragment`
- `TableClass`: CSS class (default `rz-grid-table`)
