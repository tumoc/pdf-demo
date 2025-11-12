using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;

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
              : "7");
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
                //col.Item().Component(new SectionTracker("overview", TrackSection));
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
                //col.Item().Component(new SectionTracker("overview_table", TrackSection));
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
                //col.Item().Component(new SectionTracker("case_list", TrackSection));
                col.Item().Text("3. DANH SÁCH CHI TIẾT CASE").Bold().FontSize(16);

                int i = 1;
                foreach (var c in _data.CaseDetailExports)
                {
                  //col.Item().Component(new SectionTracker($"case_{i}", TrackSection));
                  col.Item().PaddingTop(5).Text($"B{i}: {c.Name}");
                  i++;
                }
              });

        page.Footer().AlignRight().Text(x => x.CurrentPageNumber());
      });
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
      using (var ms = new MemoryStream())
      {
        Document.Create(c => Compose(c)).GeneratePdf(ms);
      }

      Document.Create(c => Compose(c)).GeneratePdf(filePath);
    }
  }
}
