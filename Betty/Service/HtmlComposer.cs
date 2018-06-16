using Betty.Auth;
using Betty.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public class HtmlComposer : IHtmlComposer
    {
        private readonly HtmlDocument _doc;
        //Keeps track of important nodes
        private HtmlNode _body;
        private HtmlNode _head;
        private HtmlNode _html;
        public HtmlComposer()
        {
            _doc = new HtmlAgilityPack.HtmlDocument();
            Init();
        }
        public void AppendText(string tag, string text)
        {
            var textNode = _doc.CreateTextNode(text);
            var p = _doc.CreateElement(tag);
            p.AppendChild(textNode);
            _body.AppendChild(p);
        }
        private void Init()
        {
            _html = _doc.CreateElement("html");
            _body = _doc.CreateElement("body");
            _head = _doc.CreateElement("head");
            _html.AppendChild(_head);
            _html.AppendChild(_body);
            _doc.DocumentNode.AppendChild(_html);

        }
        public override string ToString()
        {
            return _doc.DocumentNode.WriteTo();
        }
    }
}