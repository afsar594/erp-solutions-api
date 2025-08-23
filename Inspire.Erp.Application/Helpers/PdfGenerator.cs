//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using PdfSharpCore.Drawing;

//namespace Inspire.Erp.Application.Helpers
//{
//    public static class PdfGenerator
//    {

//        public static FileContentResult BuildDayEndReportPdf(PosDayEndReportDto reportDto)
//        {
//            var doc = CreateBasicPdf("POS Day End Report", out var gfx, out var font, out var y);

//            foreach (var ds in reportDto.DailySummary)
//            {
//                gfx.DrawString($"Gross Sales: {ds.GrossAmount:N2}", font, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString($"Discounts: {ds.DiscountAmt:N2}", font, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString($"Net Sales: {ds.NetAmount:N2}", font, XBrushes.Black, 40, y); y += 30;
//            }

//            gfx.DrawString("Top Items:", font, XBrushes.Black, 40, y); y += 20;

//            foreach (var item in reportDto.TopItems)
//            {
//                gfx.DrawString($"{item.Code} - {item.ItemName} x{item.QtySold} @ {item.Rate:N2} = {item.Total:N2}", font, XBrushes.Black, 50, y);
//                y += 20;
//            }

//            return FinalizePdf(doc, "DayEndReport");
//        }

//        public static FileContentResult BuildItemWiseReportPdf(List<CategoryWIthItemsDto> data)
//        {
//            var doc = CreateBasicPdf("Item Wise Sales Report", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            foreach (var category in data)
//            {
//                gfx.DrawString($"Category: {category.CategoryName}", boldFont, XBrushes.Black, 40, y);
//                y += 25;

//                // Table Headers
//                gfx.DrawString("Item Name", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Qty", boldFont, XBrushes.Black, 300, y);
//                gfx.DrawString("Total", boldFont, XBrushes.Black, 400, y);
//                y += 20;

//                // Line under headers
//                //gfx.DrawLine(XPens.Black, 60, y, 500, y);
//                y += 5;

//                foreach (var item in category.GroupCategories.SelectMany(x => x.CategoryItems))
//                {
//                    gfx.DrawString(item.ItemName, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(item.Qty.ToString("N2"), font, XBrushes.Black, 300, y);
//                    gfx.DrawString(item.Total.ToString("N2"), font, XBrushes.Black, 400, y);
//                    y += 20;

//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }

//                y += 15; // Spacing between categories
//            }

//            return FinalizePdf(doc, "ItemWiseReport");
//        }


//        public static FileContentResult BuildTransactionWiseReportPdf(List<DailySettlementSummary> data)
//        {
//            var doc = CreateBasicPdf("Transaction Wise Report", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table Headers
//            gfx.DrawString("Date", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Description", boldFont, XBrushes.Black, 150, y);
//            gfx.DrawString("Amount", boldFont, XBrushes.Black, 450, y);
//            y += 20;

//            // Underline headers
//            gfx.DrawLine(XPens.Black, 40, y, 550, y);
//            y += 5;

//            foreach (var item in data)
//            {
//                gfx.DrawString(item.SettlementDate.ToString("yyyy-MM-dd"), font, XBrushes.Black, 40, y);
//                gfx.DrawString(item.TransDescription, font, XBrushes.Black, 150, y);
//                gfx.DrawString(item.Amount.ToString("N2"), font, XBrushes.Black, 450, y);
//                y += 20;

//                // Page break

//                if (CheckPageBreak(ref doc, ref gfx, ref y))
//                {
//                    // Redraw table headers on new page
//                    gfx.DrawString("Date", boldFont, XBrushes.Black, 40, y);
//                    gfx.DrawString("Description", boldFont, XBrushes.Black, 150, y);
//                    gfx.DrawString("Amount", boldFont, XBrushes.Black, 450, y);
//                    y += 20;
//                    gfx.DrawLine(XPens.Black, 40, y, 550, y);
//                    y += 5;
//                }
//            }

//            return FinalizePdf(doc, "TransactionWiseReport");
//        }


//        public static FileContentResult BuildItemSalesSummaryPdf(List<ItemSalesSummaryDto> data)
//        {
//            var doc = CreateBasicPdf("Item Sales Summary", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table Header
//            gfx.DrawString("Item Name", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Sold Qty", boldFont, XBrushes.Black, 200, y);
//            gfx.DrawString("Qty %", boldFont, XBrushes.Black, 280, y);
//            gfx.DrawString("Total", boldFont, XBrushes.Black, 360, y);
//            gfx.DrawString("Sales %", boldFont, XBrushes.Black, 460, y);
//            y += 20;

//            // Header underline
//            gfx.DrawLine(XPens.Black, 40, y, 550, y);
//            y += 5;

//            foreach (var item in data)
//            {
//                gfx.DrawString(item.ItemName, font, XBrushes.Black, 40, y);
//                gfx.DrawString(item.SoldQty.ToString("N2"), font, XBrushes.Black, 200, y);
//                gfx.DrawString(item.QtyPercent.ToString("N2"), font, XBrushes.Black, 280, y);
//                gfx.DrawString(item.Total.ToString("N2"), font, XBrushes.Black, 360, y);
//                gfx.DrawString(item.SalesPercent.ToString("N2"), font, XBrushes.Black, 460, y);
//                y += 20;

//                // Page break
//                if (CheckPageBreak(ref doc, ref gfx, ref y))
//                {
//                    var page = doc.AddPage();
//                    gfx = XGraphics.FromPdfPage(page);
//                    y = 40;

//                    // Redraw headers on new page
//                    gfx.DrawString("Item Name", boldFont, XBrushes.Black, 40, y);
//                    gfx.DrawString("Sold Qty", boldFont, XBrushes.Black, 200, y);
//                    gfx.DrawString("Qty %", boldFont, XBrushes.Black, 280, y);
//                    gfx.DrawString("Total", boldFont, XBrushes.Black, 360, y);
//                    gfx.DrawString("Sales %", boldFont, XBrushes.Black, 460, y);
//                    y += 20;
//                    gfx.DrawLine(XPens.Black, 40, y, 550, y);
//                    y += 5;
//                }
//            }

//            return FinalizePdf(doc, "ItemSalesSummary");
//        }

//        public static FileContentResult BuildOrderSummaryPdf(List<OrderSummaryDto> data)
//        {
//            var doc = CreateBasicPdf("Order Summary", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table Header
//            gfx.DrawString("Order #", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Time", boldFont, XBrushes.Black, 120, y);
//            gfx.DrawString("Table", boldFont, XBrushes.Black, 250, y);
//            gfx.DrawString("Covers", boldFont, XBrushes.Black, 330, y);
//            gfx.DrawString("Net Amount", boldFont, XBrushes.Black, 420, y);
//            y += 20;

//            // Underline headers
//            gfx.DrawLine(XPens.Black, 40, y, 550, y);
//            y += 5;

//            foreach (var item in data)
//            {
//                gfx.DrawString(item.SettlementId, font, XBrushes.Black, 40, y);
//                gfx.DrawString(item.OrderTime, font, XBrushes.Black, 120, y);
//                gfx.DrawString(item.Table, font, XBrushes.Black, 250, y);
//                gfx.DrawString(item.Covers.ToString(), font, XBrushes.Black, 330, y);
//                gfx.DrawString(item.NetAmount.ToString("N2"), font, XBrushes.Black, 420, y);
//                y += 20;

//                // Page break if needed
//                if (CheckPageBreak(ref doc, ref gfx, ref y))
//                {
//                    var page = doc.AddPage();
//                    gfx = XGraphics.FromPdfPage(page);
//                    y = 40;

//                    // Redraw header
//                    gfx.DrawString("Order #", boldFont, XBrushes.Black, 40, y);
//                    gfx.DrawString("Time", boldFont, XBrushes.Black, 120, y);
//                    gfx.DrawString("Table", boldFont, XBrushes.Black, 250, y);
//                    gfx.DrawString("Covers", boldFont, XBrushes.Black, 330, y);
//                    gfx.DrawString("Net Amount", boldFont, XBrushes.Black, 420, y);
//                    y += 20;
//                    gfx.DrawLine(XPens.Black, 40, y, 550, y);
//                    y += 5;
//                }
//            }

//            return FinalizePdf(doc, "OrderSummary");
//        }

//        public static FileContentResult BuildTimeWiseSalesPdf(List<OrderTimeWiseSummaryDto> data)
//        {
//            var doc = CreateBasicPdf("Time Wise Sales Summary", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table Header
//            gfx.DrawString("Time Slot", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Amount", boldFont, XBrushes.Black, 180, y);
//            gfx.DrawString("Pax", boldFont, XBrushes.Black, 280, y);
//            gfx.DrawString("Percentage", boldFont, XBrushes.Black, 340, y);
//            gfx.DrawString("Due Date", boldFont, XBrushes.Black, 450, y);
//            y += 20;

//            // Underline headers
//            gfx.DrawLine(XPens.Black, 40, y, 550, y);
//            y += 5;

//            foreach (var item in data)
//            {
//                gfx.DrawString(item.TimeStr, font, XBrushes.Black, 40, y);
//                gfx.DrawString(item.Amount.ToString("N2"), font, XBrushes.Black, 180, y);
//                gfx.DrawString(item.Pax.ToString(), font, XBrushes.Black, 280, y);
//                gfx.DrawString(item.PercVal.ToString("N2") + " %", font, XBrushes.Black, 340, y);
//                gfx.DrawString(item.DueDate, font, XBrushes.Black, 450, y);
//                y += 20;

//                // Handle page break
//                if (CheckPageBreak(ref doc, ref gfx, ref y))
//                {
//                    var page = doc.AddPage();
//                    gfx = XGraphics.FromPdfPage(page);
//                    y = 40;

//                    // Redraw header on new page
//                    gfx.DrawString("Time Slot", boldFont, XBrushes.Black, 40, y);
//                    gfx.DrawString("Amount", boldFont, XBrushes.Black, 180, y);
//                    gfx.DrawString("Pax", boldFont, XBrushes.Black, 280, y);
//                    gfx.DrawString("Percentage", boldFont, XBrushes.Black, 340, y);
//                    gfx.DrawString("Due Date", boldFont, XBrushes.Black, 450, y);
//                    y += 20;
//                    gfx.DrawLine(XPens.Black, 40, y, 550, y);
//                    y += 5;
//                }
//            }

//            return FinalizePdf(doc, "TimeWiseSales");
//        }


//        public static FileContentResult BuildShiftReportPdf(ShiftWiseSalesDto data)
//        {
//            var doc = CreateBasicPdf("Shift Wise Sales Summary", out var gfx, out var font, out var y);
//            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // SHIFT HEADER
//            gfx.DrawString($"Shift: {data.ShiftName}", boldFont, XBrushes.Black, 40, y); y += 20;

//            // SUMMARY TOTALS
//            //gfx.DrawString($"Gross: {data.GrossAmount:N2}, Net: {data.NetAmount:N2}, Discount: {data.DiscountAmount:N2}", font, XBrushes.Black, 40, y); y += 30;

//            // --- TRANSACTION SUMMARY ---
//            if (data.TransactionSummaries?.Any() == true)
//            {
//                gfx.DrawString("Transaction Summary", boldFont, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString("Description", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Amount", boldFont, XBrushes.Black, 300, y); y += 20;
//                gfx.DrawLine(XPens.Black, 60, y, 500, y); y += 5;

//                foreach (var txn in data.TransactionSummaries)
//                {
//                    gfx.DrawString(txn.Description, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(txn.Amount.ToString("N2"), font, XBrushes.Black, 300, y); y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }

//                y += 20;
//            }

//            // --- GROUPED SUMMARY ---
//            if (data.GroupedWiseSummaries?.Any() == true)
//            {
//                gfx.DrawString("Grouped Sales Summary", boldFont, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString("Item Name", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Qty", boldFont, XBrushes.Black, 300, y);
//                gfx.DrawString("Total", boldFont, XBrushes.Black, 400, y); y += 20;
//                gfx.DrawLine(XPens.Black, 60, y, 550, y); y += 5;

//                foreach (var item in data.GroupedWiseSummaries)
//                {
//                    gfx.DrawString(item.ItemName, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(item.SoldQty.ToString(), font, XBrushes.Black, 300, y);
//                    gfx.DrawString(item.GroupTotal.ToString("N2"), font, XBrushes.Black, 400, y); y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }

//                y += 20;
//            }

//            // --- INVOICE-WISE SALES ---
//            if (data.InvoiceWisesSales?.Any() == true)
//            {
//                gfx.DrawString("Invoice-wise Sales", boldFont, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString("Invoice #", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Time", boldFont, XBrushes.Black, 150, y);
//                gfx.DrawString("Table", boldFont, XBrushes.Black, 250, y);
//                gfx.DrawString("Covers", boldFont, XBrushes.Black, 330, y);
//                gfx.DrawString("Net", boldFont, XBrushes.Black, 420, y); y += 20;
//                gfx.DrawLine(XPens.Black, 60, y, 550, y); y += 5;

//                foreach (var inv in data.InvoiceWisesSales)
//                {
//                    gfx.DrawString(inv.SettlementId, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(inv.OrderTime, font, XBrushes.Black, 150, y);
//                    gfx.DrawString(inv.TBL, font, XBrushes.Black, 250, y);
//                    gfx.DrawString(inv.Covers.ToString(), font, XBrushes.Black, 330, y);
//                    gfx.DrawString(inv.NetAmount.ToString("N2"), font, XBrushes.Black, 420, y); y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }

//                y += 20;
//            }

//            // --- ITEM SUMMARY ---
//            if (data.ItemSummaries?.Any() == true)
//            {
//                gfx.DrawString("Item Summary", boldFont, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString("Item Name", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Qty", boldFont, XBrushes.Black, 300, y);
//                gfx.DrawString("Total", boldFont, XBrushes.Black, 400, y); y += 20;
//                gfx.DrawLine(XPens.Black, 60, y, 550, y); y += 5;

//                foreach (var item in data.ItemSummaries)
//                {
//                    gfx.DrawString(item.ItemName, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(item.Qty.ToString(), font, XBrushes.Black, 300, y);
//                    gfx.DrawString(item.Total.ToString("N2"), font, XBrushes.Black, 400, y); y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }

//                y += 20;
//            }

//            // --- ORDER TYPE SUMMARY ---
//            if (data.OrderTypeSummaries?.Any() == true)
//            {
//                gfx.DrawString("Order Type Summary", boldFont, XBrushes.Black, 40, y); y += 20;
//                gfx.DrawString("Order #", boldFont, XBrushes.Black, 60, y);
//                gfx.DrawString("Time", boldFont, XBrushes.Black, 150, y);
//                gfx.DrawString("Type", boldFont, XBrushes.Black, 280, y);
//                gfx.DrawString("Covers", boldFont, XBrushes.Black, 370, y);
//                gfx.DrawString("Net", boldFont, XBrushes.Black, 460, y); y += 20;
//                gfx.DrawLine(XPens.Black, 60, y, 550, y); y += 5;

//                foreach (var ot in data.OrderTypeSummaries)
//                {
//                    gfx.DrawString(ot.SettlementId, font, XBrushes.Black, 60, y);
//                    gfx.DrawString(ot.OrderTime, font, XBrushes.Black, 150, y);
//                    gfx.DrawString(ot.OrderType, font, XBrushes.Black, 280, y);
//                    gfx.DrawString(ot.Covers.ToString(), font, XBrushes.Black, 370, y);
//                    gfx.DrawString(ot.NetAmount.ToString("N2"), font, XBrushes.Black, 460, y); y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);
//                }
//            }

//            return FinalizePdf(doc, "ShiftWiseReport");
//        }
//        private static bool CheckPageBreak(ref PdfDocument doc, ref XGraphics gfx, ref double y)
//        {
//            if (y > 780)
//            {
//                var page = doc.AddPage();
//                gfx = XGraphics.FromPdfPage(page);
//                y = 40;
//                return true;
//            }
//            return false;
//        }


//        public static FileContentResult BuildBillWiseReportPdf(List<BillWiseSalesDto> data)
//        {
//            var doc = CreateBasicPdf("Bill Wise Sales Summary", out var gfx, out var font, out var y);
//            var titleFont = new XFont("Arial", 14, XFontStyle.Bold);
//            var headerFont = new XFont("Arial", 12, XFontStyle.Bold);

//            foreach (var group in data)
//            {
//                // Draw payment mode header
//                gfx.DrawString($"Payment Mode: {group.PayMode}", titleFont, XBrushes.Black, 40, y);
//                y += 25;

//                // Table headers
//                gfx.DrawString("Bill#", headerFont, XBrushes.Black, 40, y);
//                gfx.DrawString("Card#", headerFont, XBrushes.Black, 120, y);
//                gfx.DrawString("User", headerFont, XBrushes.Black, 250, y);
//                gfx.DrawString("Net Total", headerFont, XBrushes.Black, 400, y);
//                y += 20;

//                foreach (var bill in group.BillWiseSalesDetails)
//                {
//                    gfx.DrawString(bill.BillNo, font, XBrushes.Black, 40, y);
//                    gfx.DrawString(bill.CardNo ?? "-", font, XBrushes.Black, 120, y);
//                    gfx.DrawString(bill.User, font, XBrushes.Black, 250, y);
//                    gfx.DrawString($"{bill.Total:N2}", font, XBrushes.Black, 400, y);
//                    y += 20;
//                    CheckPageBreak(ref doc, ref gfx, ref y);

//                }

//                // Draw total for this group
//                y += 10;
//                gfx.DrawString($"Subtotal ({group.PayMode}): {group.Total:N2}", headerFont, XBrushes.Black, 40, y);
//                y += 30;
//            }

//            return FinalizePdf(doc, "BillWiseReport");
//        }

//        public static FileContentResult BuildItemCategoryReportPdf(List<ItemCategoryGroupDto> data)
//        {
//            var doc = CreateBasicPdf("Item Category Wise Summary", out var gfx, out var font, out var y);
//            var headerFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table headers
//            gfx.DrawString("Category", headerFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Quantity", headerFont, XBrushes.Black, 250, y);
//            gfx.DrawString("Total Sales", headerFont, XBrushes.Black, 400, y);
//            y += 20;

//            decimal grandQty = 0, grandTotal = 0;

//            foreach (var category in data)
//            {
//                gfx.DrawString(category.Name, font, XBrushes.Black, 40, y);
//                gfx.DrawString($"{category.TotalQty:N2}", font, XBrushes.Black, 250, y);
//                gfx.DrawString($"{category.TotalSales:N2}", font, XBrushes.Black, 400, y);
//                y += 20;

//                grandQty += category.TotalQty;
//                grandTotal += category.TotalSales;

//                CheckPageBreak(ref doc, ref gfx, ref y);
//            }

//            y += 10;
//            gfx.DrawString("Grand Total", headerFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{grandQty:N2}", headerFont, XBrushes.Black, 250, y);
//            gfx.DrawString($"{grandTotal:N2}", headerFont, XBrushes.Black, 400, y);

//            return FinalizePdf(doc, "ItemCategoryReport");
//        }


//        public static FileContentResult BuildItemGroupReportPdf(List<ItemCategoryGroupDto> data)
//        {
//            var doc = CreateBasicPdf("Item Group Wise Summary", out var gfx, out var font, out var y);
//            var headerFont = new XFont("Arial", 12, XFontStyle.Bold);

//            // Table headers
//            gfx.DrawString("Item Group", headerFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Quantity", headerFont, XBrushes.Black, 250, y);
//            gfx.DrawString("Total Sales", headerFont, XBrushes.Black, 400, y);
//            y += 20;

//            decimal grandQty = 0, grandTotal = 0;

//            foreach (var group in data)
//            {
//                gfx.DrawString(group.Name, font, XBrushes.Black, 40, y);
//                gfx.DrawString($"{group.TotalQty:N2}", font, XBrushes.Black, 250, y);
//                gfx.DrawString($"{group.TotalSales:N2}", font, XBrushes.Black, 400, y);
//                y += 20;

//                grandQty += group.TotalQty;
//                grandTotal += group.TotalSales;

//                CheckPageBreak(ref doc, ref gfx, ref y);
//            }

//            y += 10;
//            gfx.DrawString("Grand Total", headerFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{grandQty:N2}", headerFont, XBrushes.Black, 250, y);
//            gfx.DrawString($"{grandTotal:N2}", headerFont, XBrushes.Black, 400, y);

//            return FinalizePdf(doc, "ItemGroupReport");
//        }

//        public static FileContentResult BuildZReportPdf(ZReportDto data)
//        {
//            var doc = CreateBasicPdf("Z Report", out var gfx, out var font, out var y);
//            var headerFont = new XFont("Arial", 14, XFontStyle.Bold);
//            var subHeaderFont = new XFont("Arial", 12, XFontStyle.Bold);
//            var boldFont = new XFont("Arial", 10, XFontStyle.Bold);

//            // Report Header
//            gfx.DrawString("Z Report", headerFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.RepotrDate}", boldFont, XBrushes.Black, 400, y);
//            y += 30;

//            // SALES SUMMARY
//            gfx.DrawString("SALES AND TAXES SUMMARY", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString($"Total Net Sales", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.SalesSummary.TotalSales:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString($"Tax", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.SalesSummary.Tax:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString($"Total Sales", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.SalesSummary.TotalSales:N2}", boldFont, XBrushes.Black, 400, y); y += 30;

//            // SALES CATEGORY
//            gfx.DrawString("SALES CATEGORIES", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString("Category", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Quantity", boldFont, XBrushes.Black, 250, y);
//            gfx.DrawString("Net Sales", boldFont, XBrushes.Black, 400, y); y += 20;

//            foreach (var group in data.SalesCategories)
//            {
//                gfx.DrawString(group.Category, font, XBrushes.Black, 40, y);
//                gfx.DrawString($"({group.Quantity:N0})", font, XBrushes.Black, 250, y);
//                gfx.DrawString($"{group.NetSales:N2}", font, XBrushes.Black, 400, y);
//                y += 20;
//                CheckPageBreak(ref doc, ref gfx, ref y);
//            }

//            // gfx.DrawString("Total Net Sales", boldFont, XBrushes.Black, 40, y);
//            //gfx.DrawString($"{data.NetSales:N2}", boldFont, XBrushes.Black, 400, y); y += 30;

//            // PAYMENT DETAILS
//            gfx.DrawString("PAYMENT DETAILS", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString("Cash", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.PaymentDetails.Cash:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Credit", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.PaymentDetails.Credit:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Giftcard", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.PaymentDetails.Giftcard:N2}", font, XBrushes.Black, 400, y); y += 20;

//            gfx.DrawString("Total Payments", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.PaymentDetails.TotalPayments:N2}", boldFont, XBrushes.Black, 400, y); y += 20;

//            // gfx.DrawString("Total Payments - Total Sales =", font, XBrushes.Black, 40, y);
//            //gfx.DrawString($"{data.PaymentDetails.PaymentsMinusSales:N2}", font, XBrushes.Black, 400, y); y += 30;

//            // SERVER TIPOOUTS
//            gfx.DrawString("SERVER TIPOUTS", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString("Total Cash Payments", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.TotalCashPayments:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Cash Adjustments", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.CashAdjustments:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Cash before Tipouts", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.CashBeforeTipouts:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Cash Gratuity", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.CashGratuity:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Credit/Non-Cash gratuity", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.CreditNonCashGratuity:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Credit/Non-Cash tips", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.CreditNonCashTips:N2}", font, XBrushes.Black, 400, y); y += 20;

//            gfx.DrawString("(Total Non-Cash Tips and Total Gratuity: " +
//                           $"{data.ServerTipouts.CreditNonCashTips + data.ServerTipouts.CashGratuity:N2})", font, XBrushes.Gray, 40, y); y += 20;

//            gfx.DrawString("Total Cash", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.ServerTipouts.TotalCash:N2}", boldFont, XBrushes.Black, 400, y); y += 30;

//            // TOTAL DISCOUNTS
//            gfx.DrawString("TOTAL DISCOUNTS", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString("Discount Name", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString("Count", boldFont, XBrushes.Black, 250, y);
//            gfx.DrawString("Amount", boldFont, XBrushes.Black, 400, y); y += 20;

//            foreach (var d in data.Discounts)
//            {
//                gfx.DrawString(d.DiscountName, font, XBrushes.Black, 40, y);
//                gfx.DrawString($"{d.Count}", font, XBrushes.Black, 250, y);
//                gfx.DrawString($"{d.Amount:N2}", font, XBrushes.Black, 400, y); y += 20;
//                CheckPageBreak(ref doc, ref gfx, ref y);
//            }

//            y += 10;
//            // CREDIT CARD BREAKDOWN
//            gfx.DrawString("CREDIT CARD BREAKDOWN", subHeaderFont, XBrushes.Black, 40, y); y += 20;
//            gfx.DrawString("Amex", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.CreditCardBreakdown.Amex:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Mastercard", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.CreditCardBreakdown.Mastercard:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Visa", font, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.CreditCardBreakdown.Visa:N2}", font, XBrushes.Black, 400, y); y += 20;
//            gfx.DrawString("Total", boldFont, XBrushes.Black, 40, y);
//            gfx.DrawString($"{data.CreditCardBreakdown.Total:N2}", boldFont, XBrushes.Black, 400, y); y += 30;

//            return FinalizePdf(doc, "ZReport");
//        }

//        // Helpers
//        private static PdfDocument CreateBasicPdf(string title, out XGraphics gfx, out XFont font, out double y)
//        {
//            var doc = new PdfDocument();
//            var page = doc.AddPage();
//            gfx = XGraphics.FromPdfPage(page);
//            font = new XFont("Arial", 12);
//            y = 40;

//            gfx.DrawString(title, new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black,
//                new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
//            y += 40;
//            return doc;
//        }

//        private static FileContentResult FinalizePdf(PdfDocument doc, string fileName)
//        {
//            using var stream = new MemoryStream();
//            doc.Save(stream, false);
//            return BuildPdfFile(stream.ToArray(), fileName);
//        }

//        public static FileContentResult BuildPdfFile(byte[] pdfBytes, string fileName)
//        {
//            return new FileContentResult(pdfBytes, "application/pdf")
//            {
//                FileDownloadName = $"{fileName}.pdf"
//            };
//        }



//    }
//}

using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing.Layout;
using System;
using System.IO;

namespace Inspire.Erp.Application.Helpers
{
    public static class PdfGenerator
    {
        public static byte[] GeneratePdf(string title, string bodyText)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = "Untitled";

            using var ms = new MemoryStream();
            using var document = new PdfDocument();

            document.Info.Title = title;

            // Create page
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Fonts
            var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
            var bodyFont = new XFont("Arial", 12, XFontStyle.Regular);

            // Draw Title (centered)
            var titleFormat = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Near
            };
            gfx.DrawString(title, titleFont, XBrushes.Black,
                new XRect(0, 20, page.Width, 40), titleFormat);

            // Draw Body (wrapped text, left aligned)
            var tf = new XTextFormatter(gfx)
            {
                Alignment = XParagraphAlignment.Left
            };
            var rect = new XRect(40, 80, page.Width - 80, page.Height - 100);
            tf.DrawString(bodyText ?? "", bodyFont, XBrushes.Black, rect);

            // Save PDF to memory
            document.Save(ms, false);
            return ms.ToArray();
        }
    }
}


