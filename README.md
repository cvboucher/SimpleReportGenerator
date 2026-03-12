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

# ReportGenerator Component

A generic Blazor component for generating HTML table-based reports with support for grouping, aggregation, headers, footers, and detail rows.

## Overview

The `ReportGenerator<TItem>` component provides a declarative way to create grouped reports in Blazor applications. It supports:

- **Detail rows** for displaying individual data records
- **Row templates** for simple row rendering without a context wrapper
- **Group header** that displays when a group begins (one template for all levels)
- **Group footer** that displays when a group ends with aggregated values (one template for all levels)
- **Multiple grouping levels** (nested groups)
- **Automatic aggregation** of numeric values
- **Custom aggregation and reset logic**
- **Level-aware templates** - use `context.Level` to conditionally render different content per level

## Basic Usage

```razor
@page "/my-report"

<ReportGenerator TItem="SalesData" 
                 Data="@salesData"
                 GroupBreaks="@groupBreaks">
    <TableHeader>
        <tr>
            <th>Region</th>
            <th>Product</th>
            <th class="text-end">Amount</th>
        </tr>
    </TableHeader>
    <DetailRow Context="ctx">
        <td>@ctx.Row.Region</td>
        <td>@ctx.Row.Product</td>
        <td class="text-end">@ctx.Row.Amount.ToString("C")</td>
    </DetailRow>
    <GroupHeader Context="ctx">
        <td colspan="3">
            <strong>Region: @ctx.Aggregate.Region</strong>
        </td>
    </GroupHeader>
    <GroupFooter Context="ctx">
        @if (ctx.Level == 0)
        {
            <td colspan="2"><strong>Grand Total:</strong></td>
            <td class="text-end"><strong>@ctx.Aggregate.Amount.ToString("C")</strong></td>
        }
        else
        {
            <td colspan="2">Region Total:</td>
            <td class="text-end">@ctx.Aggregate.Amount.ToString("C")</td>
        }
    </GroupFooter>
</ReportGenerator>

@code {
    private List<SalesData> salesData = new();
    
    private Dictionary<int, Func<SalesData, SalesData, bool>> groupBreaks = new()
    {
        { 0, (prev, curr) => false }, // Grand total - breaks on last row automatically
        { 1, (prev, curr) => prev.Region != curr.Region } // Break when region changes
    };

    public class SalesData
    {
        public string Region { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
```

## Group Levels

Group levels determine the nesting hierarchy of headers and footers:

| Level | Description |
|-------|-------------|
| **0** | Grand totals - only triggers on the last row |
| **1** | Innermost group - immediately surrounds detail rows |
| **2** | Next outer group - surrounds Level 1 |
| **3** | Surrounds Level 2 |
| **...** | And so on |

```
Level 2 Header (context.Level == 2)
    Level 1 Header (context.Level == 1)
        Detail Row
        Detail Row
    Level 1 Footer (context.Level == 1)
    Level 1 Header (context.Level == 1)
        Detail Row
    Level 1 Footer (context.Level == 1)
Level 2 Footer (context.Level == 2)
Level 0 Footer (context.Level == 0, Grand Totals)
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `IEnumerable<TItem>?` | **Required** | The data source for the report (alias: `Items`) |
| `Items` | `IEnumerable<TItem>?` | **Required** | Alternate name for `Data` |
| `TableCssClass` | `string?` | `null` | CSS classes for the `<table>` element (alias: `TableClass`) |
| `TableClass` | `string` | `"table table-sm table-striped table-bordered w-auto"` | Default table CSS when `TableCssClass` is not provided |
| `TableHeader` | `RenderFragment?` | `null` | Template for the `<thead>` section (alias: `ReportHeader`) |
| `ReportHeader` | `RenderFragment?` | `null` | Alternate name for `TableHeader` |
| `DetailRow` | `RenderFragment<ReportDetailContext<TItem>>?` | `null` | Template for each detail row |
| `RowTemplate` | `RenderFragment<TItem>?` | `null` | Simpler row template without context wrapper |
| `GroupHeader` | `RenderFragment<ReportGroupContext<TItem>>?` | `null` | Single template for all group headers |
| `GroupFooter` | `RenderFragment<ReportGroupContext<TItem>>?` | `null` | Single template for all group footers |
| `NoRecordsTemplate` | `RenderFragment?` | `null` | Template displayed when there are no records (alias: `EmptyTemplate`) |
| `EmptyTemplate` | `RenderFragment?` | `null` | Alternate name for `NoRecordsTemplate` |
| `GroupAggregate` | `Action<TItem, TItem>?` | `null` | Custom aggregation function |
| `GroupReset` | `Action<TItem>?` | `null` | Custom reset function for aggregates |
| `GroupBreaks` | `Dictionary<int, Func<TItem, TItem, bool>>?` | `null` | Functions that determine when groups break |
| `ReportName` | `string` | `"Report"` | Used by exporters (if any) |

## Context Objects

### ReportDetailContext<TItem>

Passed to the `DetailRow` template:

| Property | Type | Description |
|----------|------|-------------|
| `Row` | `TItem` | The current detail row data |
| `GroupStyle` | `string` | CSS style for group indentation |

### ReportGroupContext<TItem>

Passed to `GroupHeader` and `GroupFooter` templates:

| Property | Type | Description |
|----------|------|-------------|
| `Aggregate` | `TItem` | The aggregate object for this group level |
| `Row` | `TItem` | The current detail row (for reference) |
| `Level` | `int` | The group level being rendered (0 = grand total, 1+ = group levels) |
| `GroupStyle` | `string` | CSS style for group indentation |

## Using context.Level for Conditional Rendering

Since the header and footer templates are called for each breaking level, use `context.Level` to customize the output:

```razor
<GroupHeader Context="ctx">
    @switch (ctx.Level)
    {
        case 2:
            <td colspan="3" class="table-primary"><strong>Region: @ctx.Aggregate.Region</strong></td>
            break;
        case 1:
            <td colspan="3" style="@ctx.GroupStyle">Department: @ctx.Aggregate.Department</td>
            break;
    }
</GroupHeader>

<GroupFooter Context="ctx">
    @{
        var label = ctx.Level switch
        {
            0 => "Grand Total:",
            1 => "Department Total:",
            2 => "Region Total:",
            _ => $"Level {ctx.Level} Total:"
        };
    }
    <td colspan="2" style="@ctx.GroupStyle">@label</td>
    <td class="text-end">@ctx.Aggregate.Salary.ToString("C")</td>
</GroupFooter>
```

## Aggregation

### Default Aggregation

When no `GroupAggregate` function is provided, the component uses default aggregation:

- **Numeric types** (`int`, `long`, `double`, `decimal`, etc.): Values are summed
- **Non-numeric types** (`string`, `DateTime`, etc.): Values are copied from the detail row to the aggregate

### Custom Aggregation

Provide a custom `GroupAggregate` function for more control:

```razor
<ReportGenerator TItem="SalesData" 
                 Data="@data"
                 GroupAggregate="@AggregateData"
                 GroupReset="@ResetAggregate">
    ...
</ReportGenerator>

@code {
    private void AggregateData(SalesData aggregate, SalesData detail)
    {
        aggregate.TotalAmount += detail.Amount;
        aggregate.ItemCount++;
        aggregate.AverageAmount = aggregate.TotalAmount / aggregate.ItemCount;
        aggregate.Region = detail.Region; // Copy grouping field
    }

    private void ResetAggregate(SalesData aggregate)
    {
        aggregate.TotalAmount = 0;
        aggregate.ItemCount = 0;
        aggregate.AverageAmount = 0;
    }
}
```

## Group Breaks

The `GroupBreaks` dictionary defines when groups should break. Each entry maps a level to a function that compares the previous and current rows:

```razor
private Dictionary<int, Func<MyData, MyData, bool>> groupBreaks = new()
{
    // Level 0: Grand totals - automatically breaks on last row
    { 0, (prev, curr) => false },
    
    // Level 1: Break when Department changes
    { 1, (prev, curr) => prev.Department != curr.Department },
    
    // Level 2: Break when Region changes
    { 2, (prev, curr) => prev.Region != curr.Region }
};
```

**Important:**
- Level 0 always triggers a footer on the last row (grand totals)
- Headers at a level trigger when any break at that level or lower occurs
- Footers at a level trigger when any break at that level or lower occurs

## Multi-Level Grouping Example

```razor
<ReportGenerator TItem="EmployeeData" 
                 Data="@employees"
                 GroupBreaks="@groupBreaks"
                 TableCssClass="table table-sm table-bordered">
    <TableHeader>
        <tr class="table-dark">
            <th>Region</th>
            <th>Department</th>
            <th>Employee</th>
            <th class="text-end">Salary</th>
        </tr>
    </TableHeader>
    <DetailRow Context="ctx">
        <td>@ctx.Row.Region</td>
        <td>@ctx.Row.Department</td>
        <td>@ctx.Row.Name</td>
        <td class="text-end">@ctx.Row.Salary.ToString("C")</td>
    </DetailRow>
    <GroupHeader Context="ctx">
        @if (ctx.Level == 2)
        {
            <td colspan="4" class="table-primary"><strong>Region: @ctx.Aggregate.Region</strong></td>
        }
        else if (ctx.Level == 1)
        {
            <td colspan="4" class="table-secondary"><strong>Department: @ctx.Aggregate.Department</strong></td>
        }
    </GroupHeader>
    <GroupFooter Context="ctx">
        @if (ctx.Level == 1)
        {
            <td colspan="3">Department Total</td>
            <td class="text-end">@ctx.Aggregate.Salary.ToString("C")</td>
        }
        else if (ctx.Level == 2)
        {
            <td colspan="3"><strong>Region Total</strong></td>
            <td class="text-end"><strong>@ctx.Aggregate.Salary.ToString("C")</strong></td>
        }
        else if (ctx.Level == 0)
        {
            <td colspan="3"><strong>Grand Total</strong></td>
            <td class="text-end"><strong>@ctx.Aggregate.Salary.ToString("C")</strong></td>
        }
    </GroupFooter>
</ReportGenerator>

@code {
    private List<EmployeeData> employees = new();

    private Dictionary<int, Func<EmployeeData, EmployeeData, bool>> groupBreaks = new()
    {
        { 0, (prev, curr) => false },
        { 1, (prev, curr) => prev.Department != curr.Department },
        { 2, (prev, curr) => prev.Region != curr.Region }
    };

    public class EmployeeData
    {
        public string Region { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}
```
