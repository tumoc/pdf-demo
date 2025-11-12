using FashDemo.Models;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace FashDemo
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
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      _cts?.Cancel();
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
    private async Task ExportReportAsync(CancellationToken token)
    {
      var products = ProductGenerator.GenerateProducts(100);
      using var report = new Report();
      report.Load("Reports/Customreport.frx");

      var tocList = new List<TableOfContentsItem>
      {
          new() { Title = "Thông tin báo cáo", Level = 1, SectionKey = "reportinfo", PageNumber = 0 },
          new() { Title = "Thông tin sự vụ", Level = 1, SectionKey = "caseinfo", PageNumber = 0 }
      };
      report.RegisterData(tocList, "TableOfContentsItems");
      // Đăng ký data & bật datasource
      report.GetDataSource("TableOfContentsItems").Enabled = true;
      // Chuẩn bị report
      report.Prepare();
      var caseInfo = report.FindObject("CaseInfo") as ReportPage;
      //Update pagenumber
      for (int i = 0; i < report.PreparedPages.Count; i++)
      {
        var page = report.PreparedPages.GetPage(i);
        if (!string.IsNullOrEmpty(page.Tag))
        {
          var item = tocList.FirstOrDefault(x => x.SectionKey == page.Tag);
          if (item != null)
          {
            item.PageNumber = i + 1;
          }
        }
      }
      report.Prepare();


      var pdfExport = new PDFSimpleExport();
      var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
      var pdfPath = Path.Combine(desktop, "ExportReport.pdf");

      StatusText.Text = "Đang xuất PDF...";
      var sw = Stopwatch.StartNew();
      await Task.Run(() =>
      {
        report.Export(pdfExport, pdfPath);
      }, token);
      sw.Stop();
      StatusText.Text = $"✅ Xuất hoàn tất ({sw.Elapsed.TotalSeconds:F1}s) → {pdfPath}";
    }

  }
}