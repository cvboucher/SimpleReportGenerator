using Microsoft.AspNetCore.Components;

namespace SimpleReportGenerator;

/// <summary>
/// Context passed to detail row templates.
/// </summary>
public class ReportDetailContext<TItem>
{
    /// <summary>
    /// The current detail row data.
    /// </summary>
    public TItem Row { get; set; } = default!;

    /// <summary>
    /// CSS style for group indentation.
    /// </summary>
    public string GroupStyle { get; set; } = string.Empty;
}

/// <summary>
/// Context passed to group header and footer templates.
/// </summary>
public class ReportGroupContext<TItem>
{
    /// <summary>
    /// The aggregate object for this group level.
    /// </summary>
    public TItem Aggregate { get; set; } = default!;

    /// <summary>
    /// The current detail row (for reference).
    /// </summary>
    public TItem Row { get; set; } = default!;

    /// <summary>
    /// The group level being rendered (0 = grand total, 1 = innermost group, etc.).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// CSS style for group indentation.
    /// </summary>
    public string GroupStyle { get; set; } = string.Empty;
}
