using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ReportDemo.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ReportDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private CancellationTokenSource? _cts;

    public MainWindow()
    {
      InitializeComponent();
      QuestPDF.Settings.License = LicenseType.Community;
    }

    private async void BtnExport_Click(object sender, RoutedEventArgs e)
    {
      BtnExport.IsEnabled = false;
      BtnCancel.IsEnabled = true;
      StatusText.Text = "Đang tạo dữ liệu...";
      ProgressBarExport.Value = 0;

      _cts = new CancellationTokenSource();
      try
      {
        await ExportReportAsync(_cts.Token);
        StatusText.Text = "✅ Hoàn tất xuất PDF!";
      }
      catch (OperationCanceledException)
      {
        StatusText.Text = "⛔ Đã hủy tiến trình.";
      }
      catch (Exception ex)
      {
        StatusText.Text = $"❌ Lỗi: {ex.Message}";
      }
      finally
      {
        BtnExport.IsEnabled = true;
        BtnCancel.IsEnabled = false;
      }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      _cts?.Cancel();
    }
    private static byte[] CreateThumbnail(byte[] originalData, int width, int height)
    {
      using var image = SixLabors.ImageSharp.Image.Load(originalData);
      image.Mutate(x => x.Resize(width, height));
      using var ms = new MemoryStream();
      image.Save(ms, new JpegEncoder { Quality = 70 });
      return ms.ToArray();
    }
    private async Task ExportReportAsync(CancellationToken token)
    {
      var random = new Random();
      var images = Directory.GetFiles("Images")
                            .Select(path =>
                              {
                                var data = File.ReadAllBytes(path);
                                return CreateThumbnail(data, 64, 64);
                              })
                            .ToList();
      var data = new List<SaleItem>();
      for (int i = 1; i <= 10_000; i++)
      {
        token.ThrowIfCancellationRequested();
        data.Add(new SaleItem
        {
          ProductName = $"Sản phẩm #{i:D5}",
          Quantity = random.Next(1, 10),
          UnitPrice = random.Next(50_000, 2_000_000),
          ImageData = images[random.Next(images.Count)]
        });
      }

      var progress = new Progress<int>(value =>
      {
        ProgressBarExport.Value = value;
        ProgressText.Text = $"{value}%";
      });

      var report = new DemoReport("BÁO CÁO 10.000 DÒNG CÓ ẢNH", data, progress);
      string path = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
          "Report_10000Rows_Images.pdf");

      StatusText.Text = "Đang xuất PDF...";
      var sw = Stopwatch.StartNew();
      await Task.Run(() =>
      {
        report.GeneratePdf(path);
      }, token);
      sw.Stop();
      StatusText.Text = $"✅ Xuất hoàn tất ({sw.Elapsed.TotalSeconds:F1}s) → {path}";
    }
  }
}