using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using BizCover.Common.DtoModels.Certificate;
using BizCover.Common.DtoModels.Endorsement;
using BizCover.Common.Infrastructure.Logging;
using BizCover.Utility.Document.Template.Constants;
using BizCover.Utility.Document.Template.Extensions;
using BizCover.Utility.Document.Template.Services.Interfaces;
using iTextSharp.text.pdf;

namespace BizCover.Utility.Document.Template.Services.Classes
{
    public class GenerateDocumentService : IGenerateDocumentService
    {
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public GenerateDocumentService(IFileService fileService, ILogger logger)
        {
            _fileService = fileService;
            _logger = logger;
            CleanUpFile(HttpContext.Current.Server.MapPath(CertificateConstant.S_TEMPLATE_PDF_PATH), TimeSpan.FromDays(1));
            CleanUpFile(HttpContext.Current.Server.MapPath(CertificateConstant.S_SIGNATURE_IMAGE_PATH), TimeSpan.FromDays(1));
            CleanUpFile(HttpContext.Current.Server.MapPath(CertificateConstant.S_OUTPUT_PATH), TimeSpan.FromDays(1));
        }

        private bool DownloadFile(string url, string absolutePath)
        {
            _logger.LogTrace("DownloadFile :: Invoked :: url: " + url + " :: absolutePath: " + absolutePath);

            if (string.IsNullOrEmpty(absolutePath))
            {
                _logger.LogTrace("DownloadFile :: absolutePath is null. Return False.");
                return false;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    _logger.LogTrace("DownloadFile :: Open file for writing :: Path: " + absolutePath);
                    using (var output = File.OpenWrite(absolutePath))
                    {
                        _logger.LogTrace("DownloadFile :: Opened file for writing :: Path: " + absolutePath);
                        _logger.LogTrace("DownloadFile :: Get file contents to write to opened file :: File contents path: " + url);

                        using (var input = client.GetStreamAsync(url).Result)
                        {
                            _logger.LogTrace("DownloadFile :: Stream opened to url: " + url);

                            byte[] buffer = new byte[CertificateConstant.N_BYTE_LENGTH];
                            int bytesRead;

                            _logger.LogTrace("DownloadFile :: Start reading stream. :: url: " + url);

                            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                _logger.LogTrace("DownloadFile :: Write stream to file :: Buffer length: " + buffer.Length + " :: bytesRead: " + bytesRead);
                                output.Write(buffer, 0, bytesRead);
                            }

                            _logger.LogTrace("DownloadFile :: Writing to file completed.");
                        }
                    }
                }

                _logger.LogTrace("DownloadFile :: Success.");

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                return false;
            }
        }

        private string GetFileExtension(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var ext = Path.GetExtension(url);
            if (string.IsNullOrEmpty(ext))
                return null;

            var index = ext.IndexOf("?");
            if (index > 0)
                ext = ext.Substring(0, index);

            return ext;
        }

        private string GetFilename(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var filename = Path.GetFileNameWithoutExtension(url);
            return filename;
        }       

        private string GetResource(string url, string path)
        {
            _logger.LogTrace("GetResource invoked. :: url: " + url + " :: path: " + path);

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
                return null;

            var fileExt = GetFileExtension(url);
            var filename = GetFilename(url);
            var folder = HttpContext.Current.Server.MapPath(path);
            if (Directory.Exists(folder) == false)
                Directory.CreateDirectory(folder);

            var templatePdfPath = folder + filename + fileExt;

            _logger.LogTrace("GetResource templatePdfPath: " + templatePdfPath);

            CleanUpFile(folder, TimeSpan.FromDays(1));
            if (File.Exists(templatePdfPath))
            {
                _logger.LogTrace("GetResource file exists at: " + templatePdfPath);

                if (fileExt == EndorsementConstant.S_IMAGE_FILE_EXTENSION_PDF)
                {
                    _logger.LogTrace("GetResource file valid pdf.");

                    if (_fileService.IsValidPdf(templatePdfPath))
                    {
                        _logger.LogTrace("GetResource return templatePdfPath." + templatePdfPath);
                        return templatePdfPath;
                    }
                }
                else
                {
                    _logger.LogTrace("GetResource file invalid pdf. Return templatePdfPath. Return path: " + templatePdfPath);
                    return templatePdfPath;
                }
            }

            _logger.LogTrace("GetResource start download file.");

            return DownloadFile(url, templatePdfPath) ? templatePdfPath : string.Empty;
        }

        private bool CleanUpFile(string path, TimeSpan timeSpan)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            foreach (var file in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(file);
                if (DateTime.UtcNow - fileInfo.CreationTimeUtc > timeSpan || fileInfo.Length <= 0)
                    File.Delete(fileInfo.FullName);
            }
            return true;
        }

        public string GenerateCertificate(Certificate certificate)
        {
            if (certificate == null)
                return null;

            var templatePath = GetResource(certificate.TemplatePdfUrl, CertificateConstant.S_TEMPLATE_PDF_PATH);
            if (!File.Exists(templatePath))
                throw new Exception("Template pdf file not exists! :: Path: " + templatePath + " :: templatePdfUrl: " + certificate.TemplatePdfUrl);

            var filename = _fileService.GetFileName("certificate", certificate.ApplicationId, certificate.ProductId);
            var folderCertificate = HttpContext.Current.Server.MapPath(CertificateConstant.S_OUTPUT_PATH);
            var certificatePath = folderCertificate + filename;
            CleanUpFile(folderCertificate, TimeSpan.FromDays(1));

            try
            {
                var reader = new PdfReader(templatePath);
                var stamper = new PdfStamper(reader, new FileStream(certificatePath, FileMode.Create));
                stamper.SetEncryption(PdfWriter.STRENGTH128BITS, string.Empty, certificate.PdfPassword, PdfWriter.AllowPrinting | PdfWriter.AllowCopy | PdfWriter.AllowScreenReaders);
                var pdfFormFields = stamper.AcroFields;

                pdfFormFields.SetField(CertificateFieldsConstant.S_DATE, certificate.Date);

                //producer
                pdfFormFields.SetField(CertificateFieldsConstant.S_PRODUCER, certificate.Producer.FullFormat);
                pdfFormFields.SetField(CertificateFieldsConstant.S_CONTACT_NAME, certificate.Contact.Name);
                pdfFormFields.SetField(CertificateFieldsConstant.S_CONTACT_PHONE, certificate.Contact.Phone);
                pdfFormFields.SetField(CertificateFieldsConstant.S_CONTACT_FAX, certificate.Contact.Fax);
                pdfFormFields.SetField(CertificateFieldsConstant.S_CONTACT_EMAIL, certificate.Contact.Email);

                //insured
                pdfFormFields.SetField(CertificateFieldsConstant.S_INSURED_NAME, certificate.Insured.Name);
                pdfFormFields.SetField(CertificateFieldsConstant.S_INSURED_ADDRESS, certificate.Insured.AddressFormat);

                //insurer
                var insurerNumber = 1;
                foreach (var insurer in certificate.Insurers.OrderBy(o => o.Letter))
                {
                    pdfFormFields.SetField(CertificateFieldsConstant.S_INSURER_NAME + insurerNumber, insurer.Name);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_INSURER_NAIC + insurerNumber, insurer.Naic);
                    insurerNumber += 1;
                }

                var policyGL = certificate.PolicySectionGl;
                if (policyGL != null)
                {
                    //General Liability
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_INSURER_LETTER, policyGL.InsurerLetter);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_COMMERCIAL, policyGL.TakeCommercial);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_CLAIMS_MADE, policyGL.TakeClaimsMade);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_OCCURRENCE, policyGL.TakeOccur);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_OTHER_LIABILITY1, string.IsNullOrEmpty(policyGL.OtherLiability1) == false);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_OTHER_LIABILITY1_TEXT, policyGL.OtherLiability1);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_OTHER_LIABILITY2, string.IsNullOrEmpty(policyGL.OtherLiability2) == false);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_OTHER_LIABILITY2_TEXT, policyGL.OtherLiability2);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_AGGREGATE_POLICY, policyGL.TakeAggregatePolicy);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_AGGREGATE_PROJECT, policyGL.TakeAggregateProject);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_AGGREGATE_LOCATION, policyGL.TakeAggregateLoc);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_POLICY_NUMBER, policyGL.Number);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_EFFECTIVE_DATE, policyGL.EffectiveDate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_EXPIRY_DATE, policyGL.ExpiryDate);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_OCCURRENCE, policyGL.LimitOccurance);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_DAMAGE, policyGL.LimitDamage);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_MEDICAL_EXPENSE, policyGL.LimitMedical);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_PERSONAL_INJURY, policyGL.LimitPersonal);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_AGGREGATE, policyGL.LimitAggregate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_PRODUCTS, policyGL.LimitProducts);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_OTHER_TEXT, policyGL.LimitOtherText);
                    if (string.IsNullOrEmpty(policyGL.LimitOtherText) == false)
                        pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_LIMIT_OTHER_VALUE, policyGL.LimitOtherValue);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_ADDITIONAL_INSURED, policyGL.IsAdditionalInsured);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_GENERAL_LIABILITY_SUBROGATION_WAIVER, policyGL.IsAdditionalInsured);
                }


                var policyAutomobile = certificate.PolicyAutomobile;
                if (policyAutomobile != null)
                {
                    //Automobile
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_INSURER_LETTER, policyAutomobile.InsurerLetter);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_ANY_AUTO, policyAutomobile.TakeAnyAuto);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_ALL_OWNED_AUTOS, policyAutomobile.TakeAllOwnedAutos);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_SCHEDULED_OWNED_AUTOS, policyAutomobile.TakeScheduledOwnedAutos);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_HIRED_OWNED_AUTOS, policyAutomobile.TakeHiredOwnedAutos);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_NON_OWNED_AUTOS, policyAutomobile.TakeNonOwnedAutos);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_OTHER1, string.IsNullOrEmpty(policyAutomobile.Other1) == false);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_OTHER1_TEXT, policyAutomobile.Other1);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_OTHER2, string.IsNullOrEmpty(policyAutomobile.Other2) == false);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_OTHER2_TEXT, policyAutomobile.Other2);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_POLICY_NUMBER, policyAutomobile.Number);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_EFFECTIVE_DATE, policyAutomobile.EffectiveDate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_EXPIRY_DATE, policyAutomobile.ExpiryDate);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_COMBINED, policyAutomobile.LimitCombined);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_INJURY_PERSON, policyAutomobile.LimitInjuryPerson);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_INJURY_ACCIDENT, policyAutomobile.LimitInjuryAccident);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_PROPERTY_DAMAGE, policyAutomobile.LimitPropertyDamage);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_OTHER_TEXT, policyAutomobile.LimitOtherText);
                    if (string.IsNullOrEmpty(policyAutomobile.LimitOtherText) == false)
                        pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_LIMIT_OTHER, policyAutomobile.LimitOtherValue);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_ADDITIONAL_INSURED, policyAutomobile.IsAdditionalInsured);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_AUTOMOBILE_SUBROGATION_WAIVER, policyAutomobile.IsAdditionalInsured);
                }


                var policyUmbrella = certificate.PolicyUmbrella;
                if (policyUmbrella != null)
                {
                    //Umbrella
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_INSURER_LETTER, policyUmbrella.InsurerLetter);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIABILITY, policyUmbrella.TakeLiability);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_OCCURRENCE, policyUmbrella.TakeOccur);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_EXCESS, policyUmbrella.TakeExcess);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_CLAIMS_MADE, policyUmbrella.TakeClaimsMade);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_DED, policyUmbrella.TakeDed);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_RETENTION, policyUmbrella.TakeRetention);
                    if (policyUmbrella.TakeRetention)
                        pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIMIT_RETENTION, policyUmbrella.LimitiRetention);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_POLICY_NUMBER, policyUmbrella.Number);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_EFFECTIVE_DATE, policyUmbrella.EffectiveDate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_EXPIRY_DATE, policyUmbrella.ExpiryDate);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIMIT_OCCURRENCE, policyUmbrella.LimitOccurrence);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIMIT_AGGREGATE, policyUmbrella.LimitAggregate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIMIT_OTHER_TEXT, policyUmbrella.LimitOtherText);
                    if (string.IsNullOrEmpty(policyUmbrella.LimitOtherText) == false)
                        pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_LIMIT_OTHER, policyUmbrella.LimitOtherValue);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_ADDITIONAL_INSURED, policyUmbrella.IsAdditionalInsured);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_UMBRELLA_SUBROGATION_WAIVER, policyUmbrella.IsAdditionalInsured);
                }


                var policyWorker = certificate.PolicyWorker;
                if (policyWorker != null)
                {
                    //Workers Compensation and Employers's Liability
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_INSURER_LETTER, policyWorker.InsurerLetter);
                    pdfFormFields.SetFieldYesNo(CertificateFieldsConstant.S_WORKER_EXCLUDED_PARTNER, policyWorker.TakeAnyExcludedPartner);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_POLICY_NUMBER, policyWorker.Number);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_EFEECTIVATE_DATE, policyWorker.EffectiveDate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_EXPIRY_DATE, policyWorker.ExpiryDate);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_STATUTORY, policyWorker.TakeLimitStatutory);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_OTHER, policyWorker.TakeLimitOther);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_LIMIT_ACCIDENT, policyWorker.LimitAccident);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_LIMIT_EMPLOYEE, policyWorker.LimitEmployee);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_LIMIT_POLICY, policyWorker.LimitPolicy);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_WORKER_SUBROGATION_WAIVER, policyWorker.IsAdditionalInsured);
                }


                var policyOther = certificate.PolicyOther;
                if (policyOther != null)
                {
                    //Other
                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_INSURER_LETTER, policyOther.InsurerLetter);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_TYPE, policyOther.InsuranceName);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_POLICY_NUMBER, policyOther.Number);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_EFFECITVE_DATE, policyOther.EffectiveDate);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_EXPIRY_DATE, policyOther.ExpiryDate);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_LIMITS, policyOther.LimitFormat);

                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_ADDITIONAL_INSURED, policyOther.IsAdditionalInsured);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_OTHER_SUBROGATION_WAIVER, policyOther.IsAdditionalInsured);
                }


                pdfFormFields.SetField(CertificateFieldsConstant.S_DESCRIPTION, certificate.Description);

                for (var i = 0; i < certificate.DescriptionColumn.Count(); i++)
                {
                    CustomCoverage customCoverage = certificate.DescriptionColumn.ElementAt(i);
                    pdfFormFields.SetField(CertificateFieldsConstant.S_DESCRIPTIONCOLUMN + (i+1), customCoverage.FullFormat);
                }

                pdfFormFields.SetField(CertificateFieldsConstant.S_HOLDER, certificate.Holder.FullFormat);

                //add signature image
                
                var signatureFile = GetResource(certificate.SignatureImageUrl, CertificateConstant.S_SIGNATURE_IMAGE_PATH);
                stamper.SetImage(CertificateFieldsConstant.S_SIGNATURE, signatureFile);

                // flatten form fields and close document
                stamper.FormFlattening = true;
                stamper.Close();
                stamper.Dispose();
                reader.Close();
                reader.Dispose();

                return certificatePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GenerateEndorsement(EndorsementDto endorsement)
        {
            if (endorsement == null)
                return null;

            _logger.LogTrace("GenerateEndorsement invoked. ");
            
            var templatePath = GetResource(endorsement.TemplatePdfUrl, EndorsementConstant.S_TEMPLATE_PDF_PATH);

            _logger.LogTrace("GetResource done. TemplatePath: " + templatePath);

            if (!File.Exists(templatePath))
            {
                _logger.LogTrace("Endorsement file does not exist at path: " + templatePath);
                throw new Exception("Template pdf file not exists! :: Path: " + templatePath + " :: templatePdfUrl: " + endorsement.TemplatePdfUrl);
            }

            if (_fileService.IsValidPdf(templatePath) == false)
            {
                _logger.LogTrace("IsValidPdf is false :: TemplatePath: " + templatePath);
                throw new Exception("Template pdf file is invalid! :: Path: " + templatePath + " :: templatePdfUrl: " + endorsement.TemplatePdfUrl);
            }

            var filename = string.Empty;
            var folderEndorsement = string.Empty;
            var endorsementPath = string.Empty;

            _logger.LogTrace("Endorsement is set to parseemptytext: " + endorsement.ParseEmptyText);

            if (endorsement.ParseEmptyText)
            {
                filename = string.Format(CertificateConstant.S_DOCUMENT_FILENAME_EMPTY_FORMAT, EndorsementConstant.S_DOCUMENT_FILENAME_EMPTY, endorsement.EndorsementCode);

                _logger.LogTrace("Empty Endorsement filename: " + filename);

                folderEndorsement = HttpContext.Current.Server.MapPath(EndorsementConstant.S_OUTPUT_PATH_EMPTY);

                _logger.LogTrace("Empty Endorsement folder: " + folderEndorsement);

                endorsementPath = folderEndorsement + filename;

                _logger.LogTrace("Empty Endorsement path: " + endorsementPath);

                //Cleanup once a week.
                CleanUpFile(folderEndorsement, TimeSpan.FromDays(7));

                if (File.Exists(endorsementPath))
                {
                    _logger.LogTrace("Empty Endorsement exists already. Return path. " + endorsementPath);
                    return endorsementPath;
                }
            }
            else
            {
                filename = _fileService.GetFileName("endorsement", endorsement.ApplicationId, endorsement.ProductId);

                _logger.LogTrace("Endorsement file: " + filename);

                folderEndorsement = HttpContext.Current.Server.MapPath(EndorsementConstant.S_OUTPUT_PATH);

                _logger.LogTrace("Endorsement folder: " + folderEndorsement);

                CleanUpFile(folderEndorsement, TimeSpan.FromDays(1));
                endorsementPath = folderEndorsement + filename;

                _logger.LogTrace("Endorsement path: " + endorsementPath);
            }

            if (File.Exists(endorsementPath))
            {
                File.Delete(endorsementPath);
                _logger.LogTrace("Endorsement exists already. Delete file. " + endorsementPath);
            }

            var reader = new PdfReader(templatePath);
            var stamper = new PdfStamper(reader, new FileStream(endorsementPath, FileMode.Create));
            try
            {
                _logger.LogTrace("Parse actual pdf document start. ");
                stamper.SetEncryption(PdfWriter.STRENGTH128BITS, string.Empty, endorsement.PdfPassword, PdfWriter.AllowPrinting | PdfWriter.AllowCopy | PdfWriter.AllowScreenReaders);
                var pdfFormFields = stamper.AcroFields;
                var imageExtensions = new string[] { EndorsementConstant.S_IMAGE_FILE_EXTENSION_PNG, EndorsementConstant.S_IMAGE_FILE_EXTENSION_JPG };

                if (endorsement.VariableValues != null)
                {
                    foreach (var variableValue in endorsement.VariableValues)
                    {

                        if (imageExtensions.Contains(Path.GetExtension(variableValue.Value)))
                        {
                            var signatureFile = GetResource(variableValue.Value, EndorsementConstant.S_SIGNATURE_IMAGE_PATH);

                            if (!string.IsNullOrWhiteSpace(signatureFile))
                            {
                                stamper.SetImage(variableValue.Key, signatureFile);
                            }
                        }
                        else
                        {
                            pdfFormFields.SetField(variableValue.Key, variableValue.Value);
                        }
                    }
                }

                _logger.LogTrace("Parse actual pdf document done.");

                return endorsementPath;
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
            finally
            {
                // flatten form fields and close document
                stamper.FormFlattening = true;
                stamper.Close();
                stamper.Dispose();
                reader.Close();
                reader.Dispose();
            }
        }
    }
}