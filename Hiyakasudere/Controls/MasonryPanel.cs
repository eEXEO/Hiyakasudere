using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Hiyakasudere.Controls;

/// <summary>
/// A true masonry/waterfall layout panel that distributes items across columns
/// using a shortest-column-first algorithm. Each column has a fixed width,
/// and items have variable height preserving their aspect ratio.
/// </summary>
public class MasonryPanel : Panel
{
    public static readonly StyledProperty<double> ColumnWidthProperty =
        AvaloniaProperty.Register<MasonryPanel, double>(nameof(ColumnWidth), 220.0);

    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<MasonryPanel, double>(nameof(Gap), 8.0);

    public double ColumnWidth
    {
        get => GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    static MasonryPanel()
    {
        AffectsMeasure<MasonryPanel>(ColumnWidthProperty, GapProperty);
        AffectsArrange<MasonryPanel>(ColumnWidthProperty, GapProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var columnWidth = ColumnWidth;
        var gap = Gap;
        var availableWidth = double.IsInfinity(availableSize.Width) ? 1200 : availableSize.Width;

        // Calculate number of columns
        var columnCount = Math.Max(1, (int)((availableWidth + gap) / (columnWidth + gap)));
        var actualColumnWidth = (availableWidth - (columnCount - 1) * gap) / columnCount;

        // Measure each child
        foreach (var child in Children)
        {
            child.Measure(new Size(actualColumnWidth, double.PositiveInfinity));
        }

        // Calculate column heights
        var columnHeights = new double[columnCount];

        foreach (var child in Children)
        {
            var shortestCol = GetShortestColumnIndex(columnHeights);
            var childHeight = child.DesiredSize.Height;
            columnHeights[shortestCol] += childHeight + gap;
        }

        var maxHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;
        return new Size(availableWidth, Math.Max(0, maxHeight - gap));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var columnWidth = ColumnWidth;
        var gap = Gap;
        var availableWidth = finalSize.Width;

        // Calculate number of columns
        var columnCount = Math.Max(1, (int)((availableWidth + gap) / (columnWidth + gap)));
        var actualColumnWidth = (availableWidth - (columnCount - 1) * gap) / columnCount;

        // Track column heights
        var columnHeights = new double[columnCount];

        foreach (var child in Children)
        {
            var shortestCol = GetShortestColumnIndex(columnHeights);

            var x = shortestCol * (actualColumnWidth + gap);
            var y = columnHeights[shortestCol];
            var childHeight = child.DesiredSize.Height;

            child.Arrange(new Rect(x, y, actualColumnWidth, childHeight));
            columnHeights[shortestCol] += childHeight + gap;
        }

        var maxHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;
        return new Size(availableWidth, Math.Max(0, maxHeight - gap));
    }

    private static int GetShortestColumnIndex(double[] columnHeights)
    {
        var minIndex = 0;
        var minHeight = columnHeights[0];

        for (int i = 1; i < columnHeights.Length; i++)
        {
            if (columnHeights[i] < minHeight)
            {
                minHeight = columnHeights[i];
                minIndex = i;
            }
        }

        return minIndex;
    }
}
