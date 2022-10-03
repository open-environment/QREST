using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace QREST.App_Logic
{
    /// <summary>
    /// Overrides iTextSharp default HTML Tag processor to handle base64 images embedded within HTML
    /// </summary>
    public class CustomImageHTMLTagProcessor : IHTMLTagProcessor
    {
        /// <summary>
        /// Tells the HTMLWorker what to do when a close tag is encountered.
        /// </summary>
        public void EndElement(HTMLWorker worker, string tag)
        {
        }

        /// <summary>
        /// Tells the HTMLWorker what to do when an open tag is encountered.
        /// </summary>
        public void StartElement(HTMLWorker worker, string tag, IDictionary<string, string> attrs)
        {
            Image image;
            var src = attrs["src"];

            if (src.StartsWith("data:image/"))
            {
                // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                var base64Data = src.Substring(src.IndexOf(",") + 1);
                var imagedata = Convert.FromBase64String(base64Data);
                image = Image.GetInstance(imagedata);
                
                //set width cannot exceed 300, height 500
                float w = (float)image.Width;
                float h = (float)image.Height;
                if (w > 300 || h > 500)
                {
                    float w_exceed_pct = w / 300f;
                    float h_exceed_pct = h / 500f;
                    float max_exceed_pct = Math.Max(w_exceed_pct, h_exceed_pct);

                    image.ScaleAbsolute(w / max_exceed_pct, h / max_exceed_pct);
                }

                //image.ScaleAbsolute(100f, 200f);
                //image.ScaleToFit(w, w);
                //image.ScaleToFitHeight = false;
                //if (image.Width > 1000)
                //{

                //    attrs.Remove(HtmlTags.WIDTH);
                //    attrs.Add(HtmlTags.WIDTH, "400px");
                //    //image.Width = 500; 
                //}
            }
            else if (src.Contains(".webp"))  //pdf doesn't support webp images
            {
                image = null; 
            }
            else
            {
                image = Image.GetInstance(src);
            }

            worker.UpdateChain(tag, attrs);
            worker.ProcessImage(image, attrs);
            worker.UpdateChain(tag);
        }
    }
}