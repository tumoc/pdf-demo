using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System.IO;

namespace ReportDemo.Models
{
  public class DemoReport : IDocument
  {
    private readonly List<SaleItem> _data;
    private readonly string _title;
    private readonly IProgress<int>? _progress;

    public DemoReport(string title, List<SaleItem> data, IProgress<int>? progress = null)
    {
      _data = data;
      _title = title;
      _progress = progress;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      var watermarkData = CreateTransparentWatermark("Images/background.jpg", 0.4f);
      container.Page(page =>
      {
        page.Margin(40);
        page.Size(PageSizes.A4);

        page.Header().Text(_title).Bold().FontSize(18).AlignCenter();

        page.Content().Column(col =>
          {
            col.Item().Table(table =>
              {
                table.ColumnsDefinition(cols =>
                  {
                    cols.ConstantColumn(40);
                    cols.ConstantColumn(60);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                  });

                // Header
                table.Header(header =>
                  {
                    header.Cell().Text("STT").Bold();
                    header.Cell().Text("Ảnh").Bold();
                    header.Cell().Text("Tên SP").Bold();
                    header.Cell().Text("SL").Bold();
                    header.Cell().Text("Đơn giá").Bold();
                    header.Cell().Text("Thành tiền").Bold();
                  });

                int index = 1;
                int total = _data.Count;

                foreach (var item in _data)
                {
                  table.Cell().Text(index.ToString());
                  table.Cell().Element(e =>
                    {
                      if (item.ImageData != null)
                        e.Image(item.ImageData);
                      else
                        e.Placeholder();
                    });

                  table.Cell().Text(item.ProductName);
                  table.Cell().Text(item.Quantity.ToString());
                  table.Cell().Text($"{item.UnitPrice:N0}");
                  table.Cell().Text($"{item.TotalPrice:N0}");

                  index++;
                  if (index % 100 == 0) // báo tiến trình mỗi 100 dòng
                    _progress?.Report(index * 100 / total);
                }
              });
          });
        page.Background().Element(e =>
        {
          e.Rotate(45).Image(watermarkData)
           .FitArea();
        });
        page.Footer().AlignCenter().Text(x =>
          {
            x.Span("Trang ");
            x.CurrentPageNumber();
            x.Span(" / ");
            x.TotalPages();
          });
      });
    }
    private byte[] CreateTransparentWatermark(string path, float alpha = 0.1f)
    {
      using var img = SixLabors.ImageSharp.Image.Load<Rgba32>(path);
      img.Mutate(x => x.Opacity(alpha));
      using var ms = new MemoryStream();
      img.SaveAsPng(ms);
      return ms.ToArray();
    }
  }
}
