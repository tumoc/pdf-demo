// ============================================================================
//  This file includes source code or derivative work originating from
//  the QuestPDF open-source project (https://github.com/QuestPDF/QuestPDF).
//
//  Copyright (c) QuestPDF Contributors
//  Licensed under the MIT License.
//  See the LICENSE file in the project root for full license information.
//
//  Modifications and extensions in this file were made by HTSC Co., Ltd.
//  for internal use in Cravr Report Generator.
//
//  This derivative file is used in compliance with the MIT License terms.
// ============================================================================

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestDemo.InternalQuest;
using System.Windows.Controls;

namespace QuestDemo.Models
{
  /// <summary>
  /// Thành phần nhỏ giúp ghi nhận số trang thật của một section.
  /// Hoạt động ổn định trên QuestPDF 2022.12.15 (MIT).
  /// </summary>
  public class SectionTracker : IComponent
  {
    private readonly string _sectionName;
    private readonly Action<string, int> _onTracked;

    public SectionTracker(string sectionName, Action<string, int> onTracked)
    {
      _sectionName = sectionName;
      _onTracked = onTracked;
    }

    public void Compose(IContainer container)
    {
      // Dùng một chain duy nhất để tránh lỗi “multiple child elements”
      container
          .Section(_sectionName)
          .PaddingBottom(2)
          .Text(e =>
          {
            var page = QuestPdfPageTracker.TryGetCurrentPageNumber(e);
            if (page.HasValue)
              _onTracked?.Invoke(_sectionName, page.Value);
          });
    }
  }
}
