using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Helper sanitize HTML rich-text trước khi lưu/render nội dung public.
/// Chính sách: whitelist tag/attribute tối thiểu, loại bỏ script/event/style/url nguy hiểm.
/// </summary>
public static class HtmlSanitizerHelper
{
    private static readonly HashSet<string> AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "p", "br", "strong", "b", "em", "i", "u", "ul", "ol", "li", "blockquote",
        "h2", "h3", "h4", "a", "img", "figure", "figcaption", "table", "thead", "tbody",
        "tr", "th", "td"
    };

    private static readonly HashSet<string> SelfClosingTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "br", "img"
    };

    private static readonly HashSet<string> AllowedGlobalClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "image", "align-left", "align-right", "align-center", "align-justify"
    };

    private static readonly Regex TagRegex = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex TagNameRegex = new Regex(@"^<\s*/?\s*([a-zA-Z0-9]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex AttributeRegex = new Regex(
        @"([a-zA-Z_:][-a-zA-Z0-9_:.]*)\s*=\s*(?:(""([^""]*)"")|('([^']*)')|([^\s'""=<>`]+))",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Sanitize HTML rich-text bằng whitelist tag/attribute an toàn.</summary>
    public static string SanitizeRichText(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        // Nội dung có thể đã được lưu ở dạng HTML-encoded (&aacute;, &agrave;, &ndash;...).
        // Decode trước khi sanitize để tránh mỗi lần lưu lại bị encode chồng, làm TinyMCE hiển thị entity thô.
        html = DecodeHtmlEntities(html);
        html = RemoveDangerousBlocks(html);

        var sb = new StringBuilder();
        int lastIndex = 0;

        foreach (Match tagMatch in TagRegex.Matches(html))
        {
            if (tagMatch.Index > lastIndex)
                sb.Append(EncodeTextPreservingUnicode(html.Substring(lastIndex, tagMatch.Index - lastIndex)));

            string safeTag = SanitizeTag(tagMatch.Value);
            if (!string.IsNullOrEmpty(safeTag))
                sb.Append(safeTag);

            lastIndex = tagMatch.Index + tagMatch.Length;
        }

        if (lastIndex < html.Length)
            sb.Append(EncodeTextPreservingUnicode(html.Substring(lastIndex)));

        return sb.ToString().Trim();
    }

    /// <summary>Lấy text thuần từ HTML đã sanitize, dùng cho meta description/preview.</summary>
    public static string ToPlainText(string html, int maxLength = 0)
    {
        string safeHtml = SanitizeRichText(html);
        string text = Regex.Replace(safeHtml, "<[^>]+>", " ");
        text = HttpUtility.HtmlDecode(text) ?? string.Empty;
        text = Regex.Replace(text, @"\s+", " ").Trim();

        if (maxLength > 0 && text.Length > maxLength)
            return text.Substring(0, Math.Max(0, maxLength - 3)).TrimEnd() + "...";

        return text;
    }

    private static string EncodeTextPreservingUnicode(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    private static string DecodeHtmlEntities(string html)
    {
        string decoded = html;
        for (int i = 0; i < 3; i++)
        {
            string next = HttpUtility.HtmlDecode(decoded) ?? string.Empty;
            if (next == decoded) break;
            decoded = next;
        }
        return decoded;
    }

    private static string RemoveDangerousBlocks(string html)
    {
        html = Regex.Replace(html, @"<\s*(script|style|iframe|object|embed|form|input|button|textarea|select|option|meta|link|base)[^>]*>.*?<\s*/\s*\1\s*>", string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        html = Regex.Replace(html, @"<\s*(script|style|iframe|object|embed|form|input|button|textarea|select|option|meta|link|base)[^>]*/?\s*>", string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return html;
    }

    private static string SanitizeTag(string rawTag)
    {
        var nameMatch = TagNameRegex.Match(rawTag);
        if (!nameMatch.Success) return string.Empty;

        string tagName = nameMatch.Groups[1].Value.ToLowerInvariant();
        if (!AllowedTags.Contains(tagName)) return string.Empty;

        bool isClosing = Regex.IsMatch(rawTag, @"^<\s*/", RegexOptions.CultureInvariant);
        if (isClosing)
            return SelfClosingTags.Contains(tagName) ? string.Empty : "</" + tagName + ">";

        var attrs = SanitizeAttributes(tagName, rawTag);
        if (attrs == null) return string.Empty;
        if (SelfClosingTags.Contains(tagName))
            return "<" + tagName + attrs + " />";

        return "<" + tagName + attrs + ">";
    }

    private static string SanitizeAttributes(string tagName, string rawTag)
    {
        var sb = new StringBuilder();

        foreach (Match attrMatch in AttributeRegex.Matches(rawTag))
        {
            string name = attrMatch.Groups[1].Value.ToLowerInvariant();
            string value = attrMatch.Groups[3].Success ? attrMatch.Groups[3].Value
                : attrMatch.Groups[5].Success ? attrMatch.Groups[5].Value
                : attrMatch.Groups[6].Value;

            if (string.IsNullOrWhiteSpace(name) || name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                continue;

            if (name == "style" || name == "srcdoc")
                continue;

            string safeValue = SanitizeAttributeValue(tagName, name, value);
            if (safeValue == null)
                continue;

            sb.Append(' ')
              .Append(name)
              .Append("=\"")
              .Append(HttpUtility.HtmlAttributeEncode(safeValue))
              .Append('"');
        }

        if (tagName.Equals("a", StringComparison.OrdinalIgnoreCase) && sb.ToString().Contains(" target=\"_blank\""))
        {
            string attrs = sb.ToString();
            if (!attrs.Contains(" rel=\""))
                sb.Append(" rel=\"noopener noreferrer\"");
        }

        return sb.ToString();
    }

    private static string SanitizeAttributeValue(string tagName, string name, string value)
    {
        value = (value ?? string.Empty).Trim();

        if (tagName == "a")
        {
            if (name == "href") return IsSafeLink(value) ? value : null;
            if (name == "title") return value;
            if (name == "target") return value == "_blank" || value == "_self" ? value : null;
            if (name == "rel") return "noopener noreferrer";
            if (name == "class") return FilterCssClasses(value);
            return null;
        }

        if (tagName == "img")
        {
            if (name == "src") return IsSafeImage(value) ? value : null;
            if (name == "alt" || name == "title") return value;
            if ((name == "width" || name == "height") && Regex.IsMatch(value, @"^\d{1,4}$")) return value;
            if (name == "class") return FilterCssClasses(value);
            return null;
        }

        if ((tagName == "figure" || tagName == "table" || tagName == "thead" || tagName == "tbody" || tagName == "tr" || tagName == "th" || tagName == "td") && name == "class")
            return FilterCssClasses(value);

        if (name == "title") return value;
        if (name == "class") return FilterCssClasses(value);

        return null;
    }

    private static string FilterCssClasses(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var kept = new List<string>();
        foreach (string cls in value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (AllowedGlobalClasses.Contains(cls)) kept.Add(cls);
        }
        return kept.Count == 0 ? null : string.Join(" ", kept);
    }

    private static bool IsSafeLink(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.StartsWith("/", StringComparison.Ordinal) && !value.StartsWith("//", StringComparison.Ordinal)) return true;
        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri))
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeMailto;
        return false;
    }

    private static bool IsSafeImage(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.StartsWith("/Resources/Images/", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("/Content/", StringComparison.OrdinalIgnoreCase))
            return true;
        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri))
            return uri.Scheme == Uri.UriSchemeHttps;
        return false;
    }
}
