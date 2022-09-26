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
            }
            else if (src.Contains(".webp"))
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