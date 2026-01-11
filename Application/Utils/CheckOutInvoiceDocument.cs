using Application.DTO.Orders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class CheckOutInvoiceDocument : IDocument
    {
        private readonly CheckOutDTO _data;

        public CheckOutInvoiceDocument(CheckOutDTO data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header().Text("HÓA ĐƠN THANH TOÁN")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Ngày thanh toán: {_data.OrderDate?.ToString("dd/MM/yyyy HH:mm")}");
                    col.Item().Text($"Phương thức thanh toán: {_data.PaymentMethod}");
                    col.Item().Text("");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Tên SP
                            columns.RelativeColumn(2); // Số lượng
                            columns.RelativeColumn(3); // Đơn giá
                            columns.RelativeColumn(3); // Thành tiền
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Sản phẩm").SemiBold();
                            header.Cell().Element(CellStyle).Text("Số lượng").SemiBold();
                            header.Cell().Element(CellStyle).Text("Đơn giá").SemiBold();
                            header.Cell().Element(CellStyle).Text("Thành tiền").SemiBold();

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                                .Padding(5).Background(Colors.Grey.Lighten3);
                            }
                        });

                        // Body
                        foreach (var item in _data.OrderDetails)
                        {
                            table.Cell().Padding(5).Text(item.Product?.Name ?? "");
                            table.Cell().Padding(5).Text(item.Quantity?.ToString() ?? "0");
                            table.Cell().Padding(5).Text(FormatCurrency(item.UnitPrice));
                            table.Cell().Padding(5).Text(FormatCurrency(item.TotalPrice));
                        }

                        // Tổng tiền
                        table.Cell().ColumnSpan(3).Padding(5).AlignRight().Text("Tổng cộng:").SemiBold();
                        table.Cell().Padding(5).Text(FormatCurrency(_data.OrderAmount)).SemiBold();
                    });
                });

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Cảm ơn bạn đã mua hàng!").Italic().FontSize(12);
                });
            });
        }

        private static string FormatCurrency(decimal? amount)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", amount ?? 0);
        }
    }
}
