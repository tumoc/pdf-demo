using QuestDemo.Models;
using QuestPDF.Fluent;
using System.Diagnostics;
using System.Windows;
using Path = System.IO.Path;

namespace QuestDemo
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
      // Cấu hình font fallback cho tiếng Việt
      QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
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
    private async Task ExportReportAsync(CancellationToken token)
    {
      var progress = new Progress<int>(value =>
      {
        ProgressBarExport.Value = value;
        ProgressText.Text = $"{value}%";
      });

      // Tạo dữ liệu mẫu
      StatusText.Text = "Đang tạo dữ liệu...";

      var caseManagerData = await Task.Run(() => GenerateSampleData(), token);

      StatusText.Text = "Đang tạo báo cáo...";

      // Tạo báo cáo
      var report = new CaseManagerReport(caseManagerData, progress);
      string fileName = $"CaseManager_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
      string filePath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
          fileName);

      StatusText.Text = "Đang xuất PDF...";

      var sw = Stopwatch.StartNew();
      await Task.Run(() =>
      {
        report.GeneratePdfWithToc("ReportOutput.pdf");
      }, token);

      sw.Stop();
      StatusText.Text = $"✅ Xuất hoàn tất ({sw.Elapsed.TotalSeconds:F1}s) → {fileName}";
    }

    private CaseManagerExportModel GenerateSampleData()
    {
      var random = new Random();
      var model = new CaseManagerExportModel
      {
        DateGenerated = DateTime.Now,
        ExportName = "CaseManager_Report.pdf",
        TotalCase = 15,
        CaseOpen = 8,
        CaseClosed = 7,
        TotalJob = 45
      };

      // Tạo dữ liệu case mẫu
      var caseNames = new[]
      {
        "Phân tích dữ liệu người dùng",
        "Tối ưu hóa hiệu suất hệ thống",
        "Nghiên cứu thị trường",
        "Phát triển tính năng mới",
        "Kiểm tra bảo mật",
        "Cập nhật giao diện",
        "Xử lý dữ liệu lớn",
        "Tích hợp API bên thứ ba",
        "Phân tích xu hướng",
        "Tối ưu database",
        "Nghiên cứu AI/ML",
        "Cải thiện UX/UI",
        "Xử lý lỗi hệ thống",
        "Phân tích hiệu suất",
        "Nghiên cứu công nghệ mới"
      };

      var platforms = new[] { "Facebook", "Instagram", "Twitter", "LinkedIn", "TikTok", "YouTube" };
      var keywords = new[] { "marketing", "analytics", "social media", "data analysis", "user behavior", "trending" };

      for (int i = 0; i < model.TotalCase; i++)
      {
        var caseDetail = new CaseDetailExportModel
        {
          CaseId = i + 1,
          SectionKey = $"section_{i + 1}",
          Name = caseNames[i],
          Platform = { "Facebook", "Instagram", "Twitter" },
          Description = $"Mô tả chi tiết cho case {caseNames[i].ToLower()}",
          Status = i < model.CaseOpen, // 8 case đầu là mở, 7 case sau là đóng
          Keywords = string.Join(", ", keywords.Take(random.Next(2, 5))),
          Created = DateTime.Now.AddDays(-random.Next(1, 30)),
          Modified = DateTime.Now.AddDays(-random.Next(0, 7)),
          Completed = random.Next(5, 20),
          Waiting = random.Next(0, 10),
          Failed = random.Next(0, 5),
          Inprogress = random.Next(1, 8)
        };

        // Tạo job details cho mỗi case
        int jobCount = random.Next(2, 6);
        for (int j = 0; j < jobCount; j++)
        {
          var job = new JobDetailExportModel
          {
            JobId = i * 10 + j + 1,
            Platform = platforms[random.Next(platforms.Length)],
            SearchInput = $"search_input_{j + 1}",
            Target = $"target_{j + 1}",
            Created = DateTime.Now.AddDays(-random.Next(1, 20)),
            Updated = DateTime.Now.AddDays(-random.Next(0, 5)),
            Status = random.Next(0, 4) // 0: chờ, 1: đang xử lý, 2: hoàn thành, 3: thất bại
          };
          caseDetail.JobDetailExports.Add(job);
        }

        model.CaseDetailExports.Add(caseDetail);
      }

      return model;
    }
  }
}