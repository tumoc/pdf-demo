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

using System;
using System.Reflection;

namespace QuestDemo.InternalQuest
{
  /// <summary>
  /// Helper nội bộ: truy cập thông tin trang hiện tại của QuestPDF 2022.12.15
  /// bằng cơ chế reflection.
  /// </summary>
  public static class QuestPdfPageTracker
  {
    public static int? TryGetCurrentPageNumber(object canvas)
    {
      try
      {
        if (canvas == null)
          return null;

        // Step 1: lấy DocumentDrawingContext từ Canvas
        var contextField = canvas.GetType().GetField("Context",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var drawingContext = contextField?.GetValue(canvas);
        if (drawingContext == null)
          return null;

        // Step 2: lấy RenderingContext từ DocumentDrawingContext
        var renderField = drawingContext.GetType().GetField("RenderingContext",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var renderContext = renderField?.GetValue(drawingContext);
        if (renderContext == null)
          return null;

        // Step 3: lấy _currentPage từ RenderingContext
        var pageField = renderContext.GetType().GetField("_currentPage",
            BindingFlags.NonPublic | BindingFlags.Instance);
        return (int?)pageField?.GetValue(renderContext);
      }
      catch
      {
        return null;
      }
    }
  }
}
