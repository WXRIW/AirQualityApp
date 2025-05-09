using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AirQualityApp.WinUI.Controls
{
    public sealed partial class AirQualityChart : UserControl
    {
        public ISeries[] Series { get; set; }

        public ObservableCollection<ICartesianAxis> XAxes { get; set; }

        public ObservableCollection<ICartesianAxis> YAxes { get; set; }

        public AirQualityChart()
        {
            this.InitializeComponent();

            // 示例数据
            var values = new double[] { 30, 50, 20, 60, 90 };

            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = values,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 2),
                    GeometrySize = 5,
                    Name = "PM2.5"
                }
            };

            XAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis
                {
                    Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri" },
                    LabelsRotation = 15,
                    TextSize = 12,
                }
            };

            YAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis
                {
                    Labeler = value => $"{value} μg/m³",
                    TextSize = 12,
                }
            };

            DataContext = this;
        }
    }
}
