using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using People.Domain.ValueObjects;
namespace People.Infrastructure.Persistence.Converters;

internal static class PersonXml
{
    internal const string Base = "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/";

    private static readonly Lazy<XmlSchemaSet> Schemas = new(() =>
    {
        var set = new XmlSchemaSet { XmlResolver = null };
        var assembly = typeof(PersonXml).Assembly;

        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".xsd")))
        {
            using var stream = assembly.GetManifestResourceStream(name)!;

            using var reader = XmlReader.Create(
                stream, 
                new XmlReaderSettings 
                { 
                    DtdProcessing = DtdProcessing.Prohibit, 
                    XmlResolver = null 
                }
            );

            set.Add(null, reader);
        }

        set.Compile();

        return set;
    });

    internal static XElement Read(string xml, XName root)
    {
        using var reader = XmlReader.Create(
            new StringReader(xml), 
            new XmlReaderSettings 
            { 
                DtdProcessing = DtdProcessing.Prohibit, 
                XmlResolver = null 
            }
        );

        var document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);

        if (document.Root?.Name != root) 
            throw new XmlSchemaValidationException("Unexpected document root.");

        document.Validate(Schemas.Value, null);
        
        if (document.DescendantNodes().Any(n => n is XComment or XProcessingInstruction))
            throw new NotSupportedException("Comments and processing instructions cannot be represented.");

        if (document.Root.DescendantsAndSelf().Attributes().Any(
            a => !a.IsNamespaceDeclaration && !(a.Name == "date" && a.Parent!.Name == XName.Get("ContactRecord", Base + "ContactRecord"))))
        {
            throw new NotSupportedException("Unsupported attribute cannot be represented.");
        }
        
        return document.Root;
    }

    internal static string Write(XElement element)
    {
        var xml = element.ToString(SaveOptions.DisableFormatting);
        Read(xml, element.Name);
        return xml;
    }

    internal static decimal Decimal(string text)
    {
        var value = XmlConvert.ToDecimal(text);

        static string Canonical(string number)
        {
            var negative = number.StartsWith('-');

            number = number.Trim();            
            number = number.TrimStart('+', '-');
            var parts = number.Split('.');

            var whole = parts[0].TrimStart('0');
            
            var fraction = parts.Length == 2 ? parts[1].TrimEnd('0') : "";
            
            if (whole.Length == 0) 
                whole = "0";
            
            return (negative && (whole != "0" || fraction.Length != 0) ? "-" : "") + whole + (fraction.Length == 0 ? "" : "." + fraction);
        }

        if (Canonical(text) != Canonical(XmlConvert.ToString(value)))
            throw new NotSupportedException("Decimal exceeds the exact precision of the domain type.");
        return value;
    }

    internal static SurveyDate Date(string text)
    {
        var match = Regex.Match(text, @"^(\d{4}-\d{2}-\d{2})(Z|[+-]\d{2}:\d{2})?$");

        if (!match.Success) 
            throw new NotSupportedException("Date outside supported calendar range.");

        var zone = match.Groups[2].Value;
        
        int? offset = zone == "" ? null : zone == "Z" ? 0 :
            (int.Parse(zone.Substring(1, 2), CultureInfo.InvariantCulture) * 60 + int.Parse(zone.Substring(4, 2), CultureInfo.InvariantCulture)) * (zone[0] == '-' ? -1 : 1);
        
        return new(DateOnly.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture), offset);
    }

    internal static string Date(SurveyDate date)
    {
        var result = date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        if (date.OffsetMinutes is not int offset) 
            return result;

        if (Math.Abs((long)offset) > 840) 
            throw new ArgumentOutOfRangeException(nameof(date));

        return result + (offset == 0 ? "Z" : $"{(offset < 0 ? "-" : "+")}{Math.Abs(offset) / 60:00}:{Math.Abs(offset) % 60:00}");
    }
}
