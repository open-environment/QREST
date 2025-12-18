using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;


namespace QREST.App_Logic
{
    internal static class PDFHelper
    {
        internal static MemoryStream CreatePDFReportForCourse(Guid courseID)
        {
            //get array of lessons for this course
            var _course = db_Train.GetT_QREST_TRAIN_COURSE_byID(courseID);
            var _lessons = db_Train.GetT_QREST_TRAIN_LESSONS_byCourse(courseID);

            if (_lessons != null && _lessons.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create an instance of the document class which represents the PDF document itself.
                    Document document = new Document(PageSize.LETTER, 30f, 20f, 50f, 40f);
                    string title = _course.COURSE_NAME;

                    // Create an instance to the PDF file by creating an instance of the PDF Writer class using the document and the filestrem in the constructor.
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    writer.PageEvent = new HeaderFooter();

                    // Add meta information to the document
                    document.AddAuthor("ITEP");
                    document.AddCreator("Open Environment Software");
                    document.AddKeywords(title);
                    document.AddSubject("QREST Training");
                    document.AddTitle(title);

                    // Open the document to enable you to write to the document
                    document.Open();

                    //define standard font styles used in the document
                    BaseFont bfHelv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    Font font_head = new Font(bfHelv, 24, Font.NORMAL, BaseColor.BLACK);
                    Font font_lesson_head = new Font(bfHelv, 14, Font.BOLD, BaseColor.BLACK);
                    Font font_lesson_step_head = new Font(bfHelv, 12, Font.NORMAL, BaseColor.BLACK);
                    Font font_main = new Font(bfHelv, 12, Font.NORMAL, BaseColor.BLACK);
                    Font font_main_bold = new Font(bfHelv, 12, Font.BOLD, BaseColor.BLACK);
                    Font font_small = new Font(bfHelv, 9, Font.BOLD, BaseColor.DARK_GRAY);
                    Font font_small_no_bold = new Font(bfHelv, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                    Font font_small_no_bold_blue = new Font(bfHelv, 9, Font.NORMAL, BaseColor.BLUE);

                    // Add course title
                    document.Add(new Paragraph(title + Environment.NewLine, font_head));

                    // Add course description
                    document.Add(new Paragraph(_course.COURSE_DESC + Environment.NewLine + Environment.NewLine, font_small_no_bold));

                    //add header for each lesson
                    foreach (var _lesson in _lessons)
                    {
                        // ******************************** LESSON TITLE *************************************************************************************
                        PdfPCell cell1 = new PdfPCell(new Paragraph("Lesson " + _lesson.LESSON_SEQ + ": " + _lesson.LESSON_TITLE, font_lesson_head));
                        cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell1.BorderWidth = 0;
                        cell1.Padding = 10f;
                        PdfPTable table = new PdfPTable(1);
                        table.WidthPercentage = 100;
                        table.AddCell(cell1);                        
                        document.Add(table);

                        // Add lesson description
                        document.Add(new Paragraph(_lesson.LESSON_DESC + Environment.NewLine + Environment.NewLine, font_small_no_bold));

                        var _steps = db_Train.GetT_QREST_TRAIN_LESSON_STEPS_byLessonID(_lesson.LESSON_IDX);
                        foreach (var _step in _steps)
                        {
                            // ******************************** LESSON STEP TITLE *************************************************************************************
                            PdfPCell cell2 = new PdfPCell(new Paragraph("    Lesson Step " + _lesson.LESSON_SEQ + "." + _step.LESSON_STEP_SEQ, font_lesson_step_head));
                            cell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                            cell2.BorderWidth = 0;
                            cell2.Padding = 5f;
                            PdfPTable table2 = new PdfPTable(1);
                            table2.WidthPercentage = 100;
                            table2.AddCell(cell2);
                            document.Add(table2);

                            //Add html to the PDF START ******************************************
                            HTMLWorker worker = new HTMLWorker(document);
                            worker.StartDocument();

                            // Replace the built-in image processor
                            var tags = new HTMLTagProcessors();
                            tags[HtmlTags.IMG] = new CustomImageHTMLTagProcessor();
                            var styles = new StyleSheet();
                            styles.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTFAMILY, "tahoma");

                            PdfPCell pdfCell = new PdfPCell { Border = 0 };
                            pdfCell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                            using (var reader = new StringReader(_step.LESSON_STEP_DESC))
                            {
                                var parsedHtmlElements = HTMLWorker.ParseToList(reader, styles, tags, null);
                                foreach (var htmlElement in parsedHtmlElements)
                                {
                                    pdfCell.AddElement(htmlElement);
                                }
                            }
                            var table1 = new PdfPTable(1);
                            table1.AddCell(pdfCell);
                            document.Add(table1);

                            //add link or yt vid
                            if (string.IsNullOrEmpty(_step.REQUIRED_URL) == false)
                            {
                                document.Add(new Paragraph("Link to click: ", font_small_no_bold));
                                document.Add(new Paragraph(_step.REQUIRED_URL, font_small_no_bold_blue));
                            }
                            else if (string.IsNullOrEmpty(_step.REQUIRED_YT_VID) == false)
                            {
                                if (_step.REQUIRED_YT_VID.ToLower().Contains("mediaspace")) {
                                    document.Add(new Paragraph("Mediaspace video link to watch: " + _step.REQUIRED_YT_VID, font_small));
                                }
                                else
                                    document.Add(new Paragraph("Youtube video link to watch: https://www.youtube.com/embed/" + _step.REQUIRED_YT_VID + "?enablejsapi=1", font_small));
                            }
                            worker.EndDocument();
                            worker.Close();
                            //Add html to the PDF END ******************************************


                        }
                    }

                    // Close the document
                    document.Close();

                    // Close the writer instance
                    writer.Close();

                    byte[] file = ms.ToArray();
                    MemoryStream ms2 = new MemoryStream();
                    ms2.Write(file, 0, file.Length);
                    ms2.Position = 0;
                    return ms2;
                }
            }
            else
                return null;
        }

    }

    class HeaderFooter : PdfPageEventHelper {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            BaseFont bfHelv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            Font header_footer_font = new Font(bfHelv, 9, Font.NORMAL, BaseColor.DARK_GRAY);

            var _info = writer.Info;

            //set page header (but skip for 1st page) **************************************
            if (writer.PageNumber > 1)
            {
                PdfPTable tbHeader = new PdfPTable(1);
                tbHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tbHeader.DefaultCell.Border = 0;

                PdfPCell _cell = new PdfPCell(new Paragraph(_info.Get(PdfName.TITLE).ToString(), header_footer_font));
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = PdfPCell.BOTTOM_BORDER;
                tbHeader.AddCell(_cell);

                tbHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin) + 40, writer.DirectContent);
            }

            //set page footer **************************************
            PdfPTable tbFooter = new PdfPTable(1);
            tbFooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbFooter.DefaultCell.Border = 0;

            PdfPCell _cell2 = new PdfPCell(new Paragraph("Page " + writer.PageNumber, header_footer_font));
            _cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cell2.Border = PdfPCell.TOP_BORDER;
            tbFooter.AddCell(_cell2);

            tbFooter.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin) - 5, writer.DirectContent);
        }
    }
}