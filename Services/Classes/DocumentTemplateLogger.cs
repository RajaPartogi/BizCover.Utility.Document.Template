using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizCover.Utility.Document.Template.Services.Interfaces;
using log4net;

namespace BizCover.Utility.Document.Template.Services.Classes
{
    public class DocumentTemplateLogger : IDocumentTemplateLogger
    {
        private ILog logger;

        public DocumentTemplateLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
            logger = LogManager.GetLogger("UtilityDocumentTemplateLogger");
        }

        public bool WriteInfo(string message)
        {
            try
            {
                logger.Info(message);

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public bool WriteError(string message, Exception ex)
        {
            try
            {
                logger.Error(message, ex);

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }
}