using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BizCover.Utility.Document.Template.Services.Interfaces
{
    public interface IDocumentTemplateLogger
    {
        bool WriteInfo(string message);
        bool WriteError(string message, Exception ex);
    }
}