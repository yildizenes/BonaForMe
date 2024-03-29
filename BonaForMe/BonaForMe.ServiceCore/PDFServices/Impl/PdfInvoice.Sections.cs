﻿using BonaForMe.DomainCore.DTO.PDFModels;
using BonaForMe.DomainCore.DTO.PDFModels.Helpers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Linq;

namespace Invoicer.Services.Impl
{
    public partial class PdfInvoice
    {
        private void HeaderSection()
        {
            HeaderFooter header = Pdf.LastSection.Headers.Primary;

            Table table = header.AddTable();
            double thirdWidth = Pdf.PageWidth() / 3;

            table.AddColumn(ParagraphAlignment.Left, thirdWidth * 2);
            table.AddColumn();

            Row row = table.AddRow();

            if (!string.IsNullOrEmpty(Invoice.Image))
            {
                Image image = row.Cells[0].AddImage(Invoice.Image);
                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                image.Height = Invoice.ImageSize.Height;
                image.Width = Invoice.ImageSize.Width;
            }

            TextFrame frame = row.Cells[1].AddTextFrame();

            Table subTable = frame.AddTable();
            subTable.AddColumn(thirdWidth / 2);
            subTable.AddColumn(thirdWidth / 2);

            row = subTable.AddRow();
            row.Cells[0].MergeRight = 1;
            row.Cells[0].AddParagraph(Invoice.Title, ParagraphAlignment.Right, "H1-20");

            row = subTable.AddRow();
            row.Cells[0].AddParagraph("Invoice Number:", ParagraphAlignment.Left, "H2-9B-Color");
            row.Cells[1].AddParagraph(Invoice.Reference, ParagraphAlignment.Right, "H2-9");
            row.Cells[0].AddParagraph("Date:", ParagraphAlignment.Left, "H2-9B-Color");
            row.Cells[1].AddParagraph(Invoice.BillingDate.ToString("dd/MM/yyyy"), ParagraphAlignment.Right, "H2-9");
        }

        public void FooterSection()
        {
            HeaderFooter footer = Pdf.LastSection.Footers.Primary;

            Table table = footer.AddTable();

            var width = footer.Section.PageWidth() / 6;
            table.AddColumn(width * 4);
            table.AddColumn(width * 2);

            Row row = table.AddRow();
            if (!string.IsNullOrEmpty(Invoice.Footer))
            {
                Paragraph paragraph = row.Cells[0].AddParagraph(Invoice.Footer, ParagraphAlignment.Left, "H2-8");
                Hyperlink link = paragraph.AddHyperlink(Invoice.Footer, HyperlinkType.Web);
            }

            Paragraph info = row.Cells[1].AddParagraph();
            info.Format.Alignment = ParagraphAlignment.Right;
            info.Style = "H2-8";
            info.AddText("Page ");
            info.AddPageField();
            info.AddText(" of ");
            info.AddNumPagesField();
        }

        private void AddressSection()
        {
            Section section = Pdf.LastSection;

            Address leftAddress = Invoice.Client;
            Address rightAddress = Invoice.Company;

            if (Invoice.CompanyOrientation == PositionOption.Right)
                Utils.Swap<Address>(ref leftAddress, ref rightAddress);

            Table table = section.AddTable();
            table.AddColumn(ParagraphAlignment.Left, section.Document.PageWidth() / 2 - 10);
            table.AddColumn(ParagraphAlignment.Center, Unit.FromPoint(20));
            table.AddColumn(ParagraphAlignment.Left, section.Document.PageWidth() / 2 - 10);

            Row row = table.AddRow();
            row.Style = "H2-10B-Color";
            row.Shading.Color = Colors.White;

            row.Cells[0].AddParagraph(leftAddress.Title, ParagraphAlignment.Left);
            row.Cells[0].Format.Borders.Bottom = BorderLine;
            row.Cells[2].AddParagraph(rightAddress.Title, ParagraphAlignment.Left);
            row.Cells[2].Format.Borders.Bottom = BorderLine;

            row = table.AddRow();
            AddressCell(row.Cells[0], leftAddress.AddressLines);
            AddressCell(row.Cells[2], rightAddress.AddressLines);

            row = table.AddRow();
        }

        private void AddressCell(Cell cell, string[] address)
        {
            foreach (string line in address)
            {
                Paragraph name = cell.AddParagraph();
                if (line == address[0] || line.Contains("Company"))
                    name.AddFormattedText(line, "H2-10B");
                else
                    name.AddFormattedText(line, "H2-9-Grey");
            }
        }

        private void BillingSection()
        {
            Section section = Pdf.LastSection;

            Table table = section.AddTable();

            double width = section.PageWidth();
            double numericWidth = (width - 300) / 4;
            table.AddColumn(90);
            table.AddColumn(ParagraphAlignment.Center, 150);
            table.AddColumn(ParagraphAlignment.Center, numericWidth);
            //table.AddColumn(ParagraphAlignment.Center, numericWidth);
            table.AddColumn(ParagraphAlignment.Center, numericWidth);
            table.AddColumn(ParagraphAlignment.Center, 100);
            table.AddColumn(ParagraphAlignment.Center, numericWidth);

            BillingHeader(table);

            foreach (ItemRow item in Invoice.Items)
            {
                if (item.IsCampaignProduct)
                    BillingRowForCampaignProduct(table, item);
                else
                    BillingRow(table, item);
            }

            if (Invoice.Totals != null)
            {
                foreach (TotalRow total in Invoice.Totals)
                {
                    BillingTotal(table, total);
                }
            }

            table.AddRow();
        }

        private void BillingHeader(Table table)
        {
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Style = "H2-10B-Color";
            row.Shading.Color = Colors.White;
            row.TopPadding = 10;
            row.Borders.Bottom = BorderLine;

            row.Cells[0].AddParagraph("Item", ParagraphAlignment.Left);
            row.Cells[1].AddParagraph("Description", ParagraphAlignment.Center);
            row.Cells[2].AddParagraph("Per Unit", ParagraphAlignment.Center);
            row.Cells[3].AddParagraph("Qty", ParagraphAlignment.Center);
            //row.Cells[4].AddParagraph("Price", ParagraphAlignment.Center);
            row.Cells[4].AddParagraph("Value", ParagraphAlignment.Center);
            row.Cells[5].AddParagraph("VAT %", ParagraphAlignment.Center);
        }

        private void BillingRow(Table table, ItemRow item)
        {
            Row row = table.AddRow();
            row.Style = "TableRow";
            row.Shading.Color = MigraDocHelpers.BackColorFromHtml(Invoice.BackColor);

            Cell cell = row.Cells[0];
            cell.AddParagraph(item.Item, ParagraphAlignment.Left, "H2-9B");

            cell = row.Cells[1];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph(item.Description, ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[2];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("€" + item.PerUnit.ToCurrency(), ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[3];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph(item.Qty.ToString(), ParagraphAlignment.Center, "H2-9");

            //cell = row.Cells[4];
            //cell.VerticalAlignment = VerticalAlignment.Center;
            //cell.AddParagraph("€" + item.Price.ToCurrency(), ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[4];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("€" + item.ExtPrice.ToCurrency(), ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[5];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("%" + item.Vat.ToCurrency(), ParagraphAlignment.Center, "H2-9");
        }

        private void BillingRowForCampaignProduct(Table table, ItemRow item)
        {
            Row row = table.AddRow();
            row.Style = "TableRow";
            row.Shading.Color = MigraDocHelpers.BackColorFromHtml(Invoice.BackColor);

            Cell cell = row.Cells[0];
            cell.AddParagraph(item.Item, ParagraphAlignment.Left, "H2-9B");

            cell = row.Cells[1];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph(item.Description, ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[2];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("FREE", ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[3];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph(item.Qty.ToString(), ParagraphAlignment.Center, "H2-9");

            //cell = row.Cells[4];
            //cell.VerticalAlignment = VerticalAlignment.Center;
            //cell.AddParagraph("€" + item.Price.ToCurrency(), ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[4];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("FREE", ParagraphAlignment.Center, "H2-9");

            cell = row.Cells[5];
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.AddParagraph("%0.00", ParagraphAlignment.Center, "H2-9");
        }

        private void BillingTotal(Table table, TotalRow total)
        {

            table.Columns[4].Format.Alignment = ParagraphAlignment.Left;

            Row row = table.AddRow();
            row.Style = "TableRow";

            string font; Color shading;
            if (total.Inverse == true)
            {
                font = "H2-9B-Inverse";
                shading = MigraDocHelpers.TextColorFromHtml(Invoice.TextColor);
            }
            else
            {
                font = "H2-9B";
                shading = MigraDocHelpers.BackColorFromHtml(Invoice.BackColor);
            }

            Cell cell = row.Cells[4];
            cell.Shading.Color = shading;
            cell.AddParagraph(total.Name, ParagraphAlignment.Left, font);

            cell = row.Cells[5];
            cell.Shading.Color = shading;
            cell.AddParagraph(total.Value.ToCurrency(Invoice.Currency), ParagraphAlignment.Center, font);
        }

        private void PaymentSection()
        {
            if (Invoice.Details != null && Invoice.Details.Count > 0)
            {
                foreach (DetailRow detail in Invoice.Details)
                {
                    Section section = Pdf.LastSection;

                    Table table = section.AddTable();
                    table.AddColumn(Unit.FromPoint(section.Document.PageWidth()));
                    Row row = table.AddRow();


                    row.Cells[0].AddParagraph(detail.Title, ParagraphAlignment.Left, "H2-9B-Color");
                    row.Cells[0].Borders.Bottom = BorderLine;

                    row = table.AddRow();
                    TextFrame frame = row.Cells[0].AddTextFrame();

                    Table subTable = frame.AddTable();
                    subTable.AddColumn(section.Document.PageWidth() * 0.25);
                    subTable.AddColumn(section.Document.PageWidth() * 0.75);
                    row = subTable.AddRow();

                    foreach (string line in detail.Paragraphs)
                    {
                        var lineSplits = line.Split(':');
                        row.Cells[0].AddParagraph(lineSplits[0], ParagraphAlignment.Left, "H2-9B-Color");
                        row.Cells[1].AddParagraph(" : " + lineSplits[1], ParagraphAlignment.Left, "H2-9");
                    }

                    row.Cells[0].AddParagraph("", ParagraphAlignment.Left);
                    row.Cells[1].AddParagraph("", ParagraphAlignment.Left);

                }
            }

            //if (Invoice.Company.HasCompanyNumber || Invoice.Company.HasVatNumber)
            //{
            //    row = table.AddRow();
            //    Color shading = MigraDocHelpers.TextColorFromHtml(Invoice.TextColor);

            //    row.Cells[0].AddParagraph("Good remain property of World Wide foods until paid for in full.", ParagraphAlignment.Center, "H2-9B-Inverse").Format.Shading.Color = shading;

            //}
        }
    }
}
