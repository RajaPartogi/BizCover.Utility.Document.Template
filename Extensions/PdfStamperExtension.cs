using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;

namespace BizCover.Utility.Document.Template.Extensions
{
    public static class PdfStamperExtension
    {
        public static bool SetImage(this PdfStamper stamper, IEnumerable<string> fieldNames, string file)
        {
            if (fieldNames?.Any() == false || string.IsNullOrEmpty(file))
                return false;

            return SetFieldImage(stamper, fieldNames, file);
        }

        public static bool SetImage(this PdfStamper stamper, string fieldName, string file)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(file))
                return false;

            return SetFieldImage(stamper, new[] {fieldName}, file);
        }

        private static bool SetFieldImage(PdfStamper stamper, IEnumerable<string> fieldNames, string file)
        {
            if (stamper == null || fieldNames?.Any() == false || string.IsNullOrEmpty(file))
                return false;

            var acroFields = stamper.AcroFields;
            foreach (var fieldName in fieldNames)
            {
                var signatureArea = acroFields.GetFieldPositions(fieldName);
                var fieldPositions = acroFields.GetFieldPositions(fieldName);
                if (signatureArea != null && fieldPositions != null && File.Exists(file))
                {
                    var image = iTextSharp.text.Image.GetInstance(file);

                    var rect = signatureArea.First().position;
                    var logoRect = new iTextSharp.text.Rectangle(rect);

                    int page = fieldPositions[0].page;
                    var cb = stamper.GetOverContent(page);

                    image.SetAbsolutePosition(logoRect.Left, (logoRect.Top - logoRect.Height));
                    image.ScaleToFit(logoRect.Width, logoRect.Height);

                    cb.AddImage(image);
                }
            }

            return true;
        }
    }
}