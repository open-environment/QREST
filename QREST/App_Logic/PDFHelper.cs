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
                    Document document = new Document(PageSize.A4, 25, 25, 25, 25);
                    string title = _course.COURSE_NAME;

                    // Create an instance to the PDF file by creating an instance of the PDF Writer class using the document and the filestrem in the constructor.
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

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
                    Font font_head = new Font(bfHelv, 22, Font.NORMAL, BaseColor.BLACK);
                    Font font_main = new Font(bfHelv, 12, Font.NORMAL, BaseColor.BLACK);
                    Font font_main_bold = new Font(bfHelv, 12, Font.BOLD, BaseColor.BLACK);
                    Font font_small = new Font(bfHelv, 9, Font.BOLD, BaseColor.DARK_GRAY);
                    Font font_small_no_bold = new Font(bfHelv, 9, Font.NORMAL, BaseColor.DARK_GRAY);

                    // Add course title
                    document.Add(new Paragraph(title + Environment.NewLine, font_head));
                    // Add course description
                    document.Add(new Paragraph(_course.COURSE_DESC + Environment.NewLine + Environment.NewLine, font_small_no_bold));

                    //add header for each lesson
                    foreach (var _lesson in _lessons)
                    {
                        // Add lesson title
                        document.Add(new Paragraph("Lesson " + _lesson.LESSON_SEQ + ": " + _lesson.LESSON_TITLE + Environment.NewLine + Environment.NewLine, font_main));
                        // Add lesson description
                        document.Add(new Paragraph(_lesson.LESSON_DESC + Environment.NewLine + Environment.NewLine, font_small_no_bold));

                        var _steps = db_Train.GetT_QREST_TRAIN_LESSON_STEPS_byLessonID(_lesson.LESSON_IDX);
                        foreach (var _step in _steps)
                        {
                            //add lesson step header
                            document.Add(new Paragraph("Lesson Step " + _lesson.LESSON_SEQ + "." + _step.LESSON_STEP_SEQ + Environment.NewLine + Environment.NewLine, font_small));


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
}