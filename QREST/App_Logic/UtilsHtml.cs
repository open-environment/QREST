using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace QREST.App_Logic.BusinessLogicLayer
{
    /// <summary>
    /// Web-only HTML helpers. General-purpose string/conversion helpers live in QRESTModel.BLL.UtilsText.
    /// </summary>
    public static class UtilsHtml
    {
        /// <summary>
        /// Used to pass all string input in the system  - Strips all nasties from a string/html
        /// </summary>
        public static string GetSafeHtml(string html)
        {
            // Scrub html
            html = ScrubHtml(html);

            // remove unwanted html
            html = RemoveUnwantedTags(html);

            return html;
        }

        /// <summary>
        /// Takes in HTML and returns santized Html/string
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string ScrubHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // clear the flags on P so unclosed elements in P will be auto closed.
            HtmlNode.ElementsFlags.Remove("p");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var finishedHtml = html;

            // Embed Urls
            if (doc.DocumentNode != null)
            {
                // Get all the links we are going to
                var tags = doc.DocumentNode.SelectNodes("//a[contains(@href, 'youtube.com')]|//a[contains(@href, 'youtu.be')]|//a[contains(@href, 'vimeo.com')]|//a[contains(@href, 'screenr.com')]|//a[contains(@href, 'instagram.com')]");

                if (tags != null)
                {
                    // find formatting tags
                    foreach (var item in tags)
                    {
                        if (item.PreviousSibling == null)
                        {
                            // Prepend children to parent node in reverse order
                            foreach (var node in item.ChildNodes.Reverse())
                                item.ParentNode.PrependChild(node);
                        }
                        else
                        {
                            // Insert children after previous sibling
                            foreach (var node in item.ChildNodes)
                                item.ParentNode.InsertAfter(node, item.PreviousSibling);
                        }

                        // remove from tree
                        item.Remove();
                    }
                }


                //Remove potentially harmful elements
                var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
                if (nc != null)
                {
                    foreach (var node in nc)
                        node.ParentNode.RemoveChild(node, false);
                }

                //remove hrefs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                        node.SetAttributeValue("href", "#");
                }

                //remove img with refs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                        node.SetAttributeValue("src", "#");
                }

                //remove on<Event> handlers from all tags
                nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondblclick or @onload or @onunload or @onerror]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("onFocus");
                        node.Attributes.Remove("onBlur");
                        node.Attributes.Remove("onClick");
                        node.Attributes.Remove("onMouseOver");
                        node.Attributes.Remove("onMouseOut");
                        node.Attributes.Remove("onDblClick");
                        node.Attributes.Remove("onLoad");
                        node.Attributes.Remove("onUnload");
                        node.Attributes.Remove("onError");
                    }
                }

                // remove any style attributes that contain the word expression (IE evaluates this as script)
                nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                        node.Attributes.Remove("stYle");
                }

                // build a list of nodes ordered by stream position
                var pos = new NodePositions(doc);

                // browse all tags detected as not opened
                foreach (var error in doc.ParseErrors.Where(e => e.Code == HtmlParseErrorCode.TagNotOpened))
                {
                    // find the text node just before this error
                    var last = pos.Nodes.OfType<HtmlTextNode>().LastOrDefault(n => n.StreamPosition < error.StreamPosition);
                    if (last != null)
                    {
                        // fix the text; reintroduce the broken tag
                        last.Text = error.SourceText.Replace("/", "") + last.Text + error.SourceText;
                    }
                }

                finishedHtml = doc.DocumentNode.WriteTo();
            }


            return finishedHtml;
        }

        public static string RemoveUnwantedTags(string html)
        {

            var unwantedTagNames = new List<string>
            {
                "div",
                "font",
                "table",
                "tbody",
                "tr",
                "td",
                "th",
                "thead"
            };

            return RemoveUnwantedTags(html, unwantedTagNames);
        }

        public static string RemoveUnwantedTags(string html, List<string> unwantedTagNames)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            var htmlDoc = new HtmlDocument();

            // load html
            htmlDoc.LoadHtml(html);

            var tags = (from tag in htmlDoc.DocumentNode.Descendants()
                        where unwantedTagNames.Contains(tag.Name)
                        select tag).Reverse();


            // find formatting tags
            foreach (var item in tags)
            {
                if (item.PreviousSibling == null)
                {
                    // Prepend children to parent node in reverse order
                    foreach (var node in item.ChildNodes.Reverse())
                    {
                        item.ParentNode.PrependChild(node);
                    }
                }
                else
                {
                    // Insert children after previous sibling
                    foreach (var node in item.ChildNodes)
                    {
                        item.ParentNode.InsertAfter(node, item.PreviousSibling);
                    }
                }

                // remove from tree
                item.Remove();
            }

            // return transformed doc
            return htmlDoc.DocumentNode.WriteContentTo().Trim();
        }

        public class NodePositions
        {
            public NodePositions(HtmlDocument doc)
            {
                AddNode(doc.DocumentNode);
                Nodes.Sort(new NodePositionComparer());
            }

            private void AddNode(HtmlNode node)
            {
                Nodes.Add(node);
                foreach (HtmlNode child in node.ChildNodes)
                {
                    AddNode(child);
                }
            }

            private class NodePositionComparer : IComparer<HtmlNode>
            {
                public int Compare(HtmlNode x, HtmlNode y)
                {
                    return x.StreamPosition.CompareTo(y.StreamPosition);
                }
            }

            public List<HtmlNode> Nodes = new List<HtmlNode>();
        }



        public static string getFontAwesomeIconFromMIME(string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType)) return "";

            var iconMapping = new Dictionary<string, string>()
            {
                {"image", "fa-file-picture-o"},
                {"image/png", "fa-file-picture-o"},
                {"audio", "fa-youtube-play"},
                {"application/pdf", "fa-file-pdf-o"},
                {"application/msword", "fa-file-word-o"},
                {"application/vnd.ms-word", "fa-file-word-o"},
                {"application/vnd.oasis.opendocument.text", "fa-file-word-o"},
                {"application/vnd.openxmlformats-officedocument.wordprocessingml","fa-file-word-o"},
                {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "fa-file-word-o text-info" },
                {"application/vnd.ms-excel", "fa-file-excel-o"},
                {"application/vnd.openxmlformats-officedocument.spreadsheetml","fa-file-excel-o"},
                {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","fa-file-excel-o"},
                {"application/vnd.oasis.opendocument.spreadsheet", "fa-file-excel-o"},
                {"application/vnd.ms-powerpoint", "fa-file-powerpoint-o"},
                {"application/vnd.openxmlformats-officedocument.presentationml","fa-file-powerpoint-o"},
                {"application/vnd.oasis.opendocument.presentation", "fa-file-powerpoint-o"},
                {"text/plain", "fa-file-text-o"},
                {"text/html", "fa-file-code-o"},
                {"application/zip", "fa-file-archive-o"}
            };

            return iconMapping.ContainsKey(mimeType) ? iconMapping[mimeType] : "fa-file-o";
        }


    }
}
