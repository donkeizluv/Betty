using Betty.Auth;
using Betty.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public interface IHtmlComposer
    {
        void AppendText(string tag, string text);
    }
}