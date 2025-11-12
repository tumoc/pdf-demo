using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf.Advanced;

namespace QuestDemo.Models
{
  public class CaseManagerReport : IDocument
  {
    private readonly CaseManagerExportModel _data;
    private readonly IProgress<int>? _progress;

    private readonly Dictionary<string, int> _sectionPages = new();

    public CaseManagerReport(CaseManagerExportModel data, IProgress<int>? progress = null)
    {
      _data = data;
      _progress = progress;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      ComposeCover(container);
      ComposeTableOfContents(container);
      ComposeOverview(container);
      ComposeOverviewTable(container);
      ComposeCaseList(container);
    }

    // ========== COVER PAGE ==========
    private void ComposeCover(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(60);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

        page.Content().AlignCenter().Column(col =>
              {
                col.Item().Text("CRAVR REPORT").FontSize(36).Bold().FontColor("#1C4587");
                col.Item().Text("Case Management System").FontSize(18);
                col.Item().PaddingTop(50).Text($"Generated: {_data.DateGenerated:dd/MM/yyyy HH:mm}");
              });

        page.Footer().AlignCenter().Text(""); // không in số trang
      });
    }

    // ========== TABLE OF CONTENTS ==========
    private void ComposeTableOfContents(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(60);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

        page.Content().Column(col =>
              {
                col.Item().AlignCenter().Text("MỤC LỤC").FontSize(28).Bold().FontColor("#1C4587");
                col.Item().PaddingTop(20);

                col.Item().Table(table =>
                      {
                        table.ColumnsDefinition(cols =>
                          {
                            cols.RelativeColumn(4);
                            cols.ConstantColumn(40);
                          });

                        AddTocRow(table, "1. TỔNG QUAN BÁO CÁO", "overview");
                        AddTocRow(table, "2. BẢNG TỔNG QUAN CASE", "overview_table");
                        AddTocRow(table, "3. DANH SÁCH CHI TIẾT CASE", "case_list");

                        int idx = 1;
                        foreach (var c in _data.CaseDetailExports)
                        {
                          AddTocRow(table, $"   - {c.Name}", $"case_{idx}");
                          idx++;
                        }
                      });
              });

        page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
      });
    }

    private void AddTocRow(TableDescriptor table, string title, string sectionKey)
    {
      table.Cell().SectionLink(sectionKey).Text(title).FontSize(12);
      table.Cell().AlignRight().Text(
          _sectionPages.ContainsKey(sectionKey)
              ? _sectionPages[sectionKey].ToString()
              : "");
    }

    // ========== OVERVIEW ==========
    private void ComposeOverview(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(60);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

        page.Content().Column(col =>
              {
                col.Item().Component(new SectionTracker("overview", TrackSection));
                _sectionPages["overview"] = 0;
                col.Item().Text("1. TỔNG QUAN BÁO CÁO").Bold().FontSize(16);
                col.Item().PaddingTop(10)
                          .Text($"Total Case: {_data.TotalCase}, Open: {_data.CaseOpen}, Closed: {_data.CaseClosed}");
              });

        page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
      });
    }

    // ========== OVERVIEW TABLE ==========
    private void ComposeOverviewTable(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(60);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

        page.Content().Column(col =>
              {
                col.Item().Component(new SectionTracker("overview_table", TrackSection));
                _sectionPages["overview_table"] = 0;
                col.Item().Text("2. BẢNG TỔNG QUAN CASE").Bold().FontSize(16);

                col.Item().Table(t =>
                      {
                        t.ColumnsDefinition(c =>
                          {
                            for (int i = 0; i < 4; i++)
                              c.RelativeColumn(1);
                          });

                        t.Header(h =>
                          {
                            h.Cell().Text("ID").Bold();
                            h.Cell().Text("Name").Bold();
                            h.Cell().Text("Platform").Bold();
                            h.Cell().Text("Status").Bold();
                          });

                        foreach (var c in _data.CaseDetailExports)
                        {
                          t.Cell().Text(c.CaseId.ToString());
                          t.Cell().Text(c.Name);
                          t.Cell().Text(string.Join(",", c.Platform));
                          t.Cell().Text(c.Status);
                        }
                      });
              });

        page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
      });
    }

    // ========== CASE LIST ==========
    private void ComposeCaseList(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(60);
        page.Size(PageSizes.A4);
        page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

        page.Content().Column(col =>
      {
        // --- Tiêu đề danh sách ---
            col.Item().Component(new SectionTracker("case_list", TrackSection));
            _sectionPages["case_list"] = 0;
            col.Item().Text("3. DANH SÁCH CHI TIẾT CASE").Bold().FontSize(16);

            if (_data.CaseDetailExports.Any())
            {
              var firstCase = _data.CaseDetailExports.First();
              col.Item().Component(new SectionTracker("case_1", TrackSection));
              _sectionPages["case_1"] = 0;
              col.Item().PaddingTop(10).Text($"B1: {firstCase.Name}").FontSize(14).Bold();

          // ví dụ nội dung chi tiết
              if (!string.IsNullOrEmpty(firstCase.Description))
              {
                col.Item().PaddingTop(5).Text(firstCase.Description);
              }
            }
          });

        page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
      });

      // --- Các Case còn lại, mỗi cái 1 trang ---
      for (int i = 2; i <= _data.CaseDetailExports.Count; i++)
      {
        var c = _data.CaseDetailExports[i - 1];
        container.Page(page =>
        {
          page.Margin(60);
          page.Size(PageSizes.A4);
          page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.TimesNewRoman));

          page.Content().Column(col =>
          {
            col.Item().Component(new SectionTracker($"case_{i}", TrackSection));
            _sectionPages[$"case_{i}"] = 0;
            col.Item().Text($"B{i}: {c.Name}").FontSize(14).Bold();

            if (!string.IsNullOrEmpty(c.Description))
            {
              col.Item().PaddingTop(5).Text(c.Description);
            }
          });

          page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
        });
      }
    }


    private void TrackSection(string key, int page)
    {
      Debug.WriteLine($"[Track] {key} => {page}");
      if (!_sectionPages.ContainsKey(key))
        _sectionPages[key] = page;
    }

    // Render 2-pass để cập nhật mục lục
    public void GeneratePdfWithToc(string filePath)
    {
      string rawFile = Path.Combine(Path.GetTempPath(), "case_temp.pdf");

      // Bước 1: Xuất PDF bằng QuestPDF
      Document.Create(Compose).GeneratePdf(rawFile);

      // Bước 2: Đọc số trang thật của từng section
      var realPages = GetSectionPageIndexes(rawFile);
      foreach (var kv in realPages)
        _sectionPages[kv.Key] = kv.Value;

      // Bước 3: Tạo file cuối cùng có mục lục cập nhật số trang đúng
      Document.Create(Compose).GeneratePdf(filePath);

      //try { File.Delete(rawFile); } catch { }
    }

    private Dictionary<string, int> GetSectionPageIndexes(string filePath)
    {
      var map = new Dictionary<string, int>();

      try
      {
        var doc = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly);

        // Build lookup để tra nhanh ObjectID → PageIndex
        var pageLookup = new Dictionary<(int, int), int>();
        for (int i = 0; i < doc.Pages.Count; i++)
        {
          var pageRef = doc.Pages[i].Reference;
          if (pageRef != null)
          {
            var id = (pageRef.ObjectID.ObjectNumber, pageRef.ObjectID.GenerationNumber);
            pageLookup[id] = i;
          }
        }

        // Lấy NamedDest
        var catalog = doc.Internals.Catalog;
        //var names = catalog.Elements.GetDictionary("/Names");
        //if (names == null) return map;

        var dests = catalog.Elements.GetDictionary("/Dests");
        if (dests == null) return map;

        foreach (var key in dests.Elements.Keys)
        {
          string sectionName = key.ToString().Trim('/');
          if (!_sectionPages.ContainsKey(sectionName))
            continue;

          var destItem = dests.Elements[key];
          if (destItem == null) continue;

          PdfArray? arr = destItem switch
          {
            PdfReference r => r.Value as PdfArray,
            PdfArray a => a,
            _ => null
          };
          if (arr == null || arr.Elements.Count == 0)
            continue;

          if (arr.Elements[0] is not PdfReference pageRef)
            continue;

          var id = (pageRef.ObjectID.ObjectNumber, pageRef.ObjectID.GenerationNumber);
          if (pageLookup.TryGetValue(id, out var pageIndex))
            map[sectionName] = pageIndex + 1;
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[GetSectionPageIndexes] {ex}");
      }

      return map;
    }

  }
}
