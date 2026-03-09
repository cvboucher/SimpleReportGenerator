# SimpleReportGenerator

Minimal Blazor report table component with optional grouping and Excel export.

## Install

```bash
dotnet add package SimpleReportGenerator
```

## Usage (basic)

```razor
<ReportGenerator Items="items">
    <ReportHeader>
        <tr>
            <th>Name</th>
            <th>Total</th>
        </tr>
    </ReportHeader>
    <RowTemplate Context="row">
        <tr>
            <td>@row.Name</td>
            <td>@row.Total</td>
        </tr>
    </RowTemplate>
</ReportGenerator>
```

## Usage (group headers/footers)

```razor
@code {
    private Dictionary<int, Func<MyRow, MyRow, bool>> _groupBreaks = new()
    {
        { 1, (prev, current) => prev.DepartmentId != current.DepartmentId }
    };
}

<ReportGenerator Items="items" GroupBreaks="_groupBreaks">
    <ReportHeader>
        <tr>
            <th>Department</th>
            <th>Name</th>
            <th>Total</th>
        </tr>
    </ReportHeader>
    <GroupHeader Context="ctx">
        <tr class="table-secondary">
            <td colspan="3" style="@ctx.GroupStyle">Department: @ctx.Row.DepartmentName</td>
        </tr>
    </GroupHeader>
    <DetailRow Context="ctx">
        <tr>
            <td></td>
            <td>@ctx.Row.Name</td>
            <td>@ctx.Row.Total</td>
        </tr>
    </DetailRow>
    <GroupFooter Context="ctx">
        <tr class="table-light">
            <td colspan="2" style="@ctx.GroupStyle">Subtotal</td>
            <td>@ctx.Aggregate.Total</td>
        </tr>
    </GroupFooter>
</ReportGenerator>
```

## Excel export

`ReportGenerator` includes `ExportToXlsx()` which uses JS interop to download the file. Add the script in your host app:

```html
<script src="_content/SimpleReportGenerator/reportGenerator.js"></script>
```

Then call `await report.ExportToXlsx()` from your page with a `@ref`.

## Props

- `Items`: `IEnumerable<TItem>` (required)
- `ReportHeader`: optional `RenderFragment`
- `DetailRow`: optional `RenderFragment<ReportDetailContext<TItem>>`
- `RowTemplate`: optional `RenderFragment<TItem>`
- `GroupHeader`: optional `RenderFragment<ReportGroupContext<TItem>>`
- `GroupFooter`: optional `RenderFragment<ReportGroupContext<TItem>>`
- `EmptyTemplate`: optional `RenderFragment`
- `GroupBreaks`: `Dictionary<int, Func<TItem, TItem, bool>>`
- `GroupAggregate`: `Action<TItem, TItem>`
- `GroupReset`: `Action<TItem>`
- `ReportName`: string
- `TableClass`: string
