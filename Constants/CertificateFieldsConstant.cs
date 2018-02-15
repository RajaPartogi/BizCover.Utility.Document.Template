namespace BizCover.Utility.Document.Template.Constants
{
    public static class CertificateFieldsConstant
    {
        public const string S_DATE                      = "tagDate";
        public const string S_PRODUCER                  = "tagProducer";
        public const string S_CONTACT_NAME              = "ContactName";
        public const string S_CONTACT_PHONE             = "ContactPhone";
        public const string S_CONTACT_FAX               = "ContactFax";
        public const string S_CONTACT_EMAIL             = "ContactEmail";
        public const string S_INSURED_NAME              = "tagClientName";
        public const string S_INSURED_ADDRESS           = "tagInsuredAddress";

        public const string S_INSURER_NAME              = "tagInsuringCompany";
        public const string S_INSURER_NAIC              = "tagNAIC";

        public const string S_GENERAL_LIABILITY_INSURER_LETTER         = "Chk1";
        public const string S_GENERAL_LIABILITY_COMMERCIAL             = "tagCGL";
        public const string S_GENERAL_LIABILITY_CLAIMS_MADE            = "tagGLClaimsMade";
        public const string S_GENERAL_LIABILITY_OCCURRENCE             = "tagGLOccurrence";
        public const string S_GENERAL_LIABILITY_OTHER_LIABILITY1       = "Chk5";
        public const string S_GENERAL_LIABILITY_OTHER_LIABILITY1_TEXT  = "Liab1";
        public const string S_GENERAL_LIABILITY_OTHER_LIABILITY2       = "Chk5a";
        public const string S_GENERAL_LIABILITY_OTHER_LIABILITY2_TEXT  = "Liab2";
        public const string S_GENERAL_LIABILITY_AGGREGATE_POLICY       = "Chk6";
        public const string S_GENERAL_LIABILITY_AGGREGATE_PROJECT      = "Chk";
        public const string S_GENERAL_LIABILITY_AGGREGATE_LOCATION     = "ChkLoc";
        public const string S_GENERAL_LIABILITY_POLICY_NUMBER          = "tagGLPolicyNo";
        public const string S_GENERAL_LIABILITY_EFFECTIVE_DATE         = "tagGLEffectiveDate";
        public const string S_GENERAL_LIABILITY_EXPIRY_DATE            = "tagGLExpirationDate";
        public const string S_GENERAL_LIABILITY_LIMIT_OCCURRENCE       = "tagGLEachOccurence";
        public const string S_GENERAL_LIABILITY_LIMIT_DAMAGE           = "tagGLDamageRentedPremises";
        public const string S_GENERAL_LIABILITY_LIMIT_MEDICAL_EXPENSE  = "tagGLMedicalExpense";
        public const string S_GENERAL_LIABILITY_LIMIT_PERSONAL_INJURY  = "tagGLPersAdvInj";
        public const string S_GENERAL_LIABILITY_LIMIT_AGGREGATE         = "tagGLGeneralAggregate";
        public const string S_GENERAL_LIABILITY_LIMIT_PRODUCTS         = "tagGLProdOpAgg";
        public const string S_GENERAL_LIABILITY_LIMIT_OTHER_TEXT       = "GLBlankText";
        public const string S_GENERAL_LIABILITY_LIMIT_OTHER_VALUE      = "GLBlankLimit";
        public const string S_GENERAL_LIABILITY_ADDITIONAL_INSURED     = "tagAIGL";
        public const string S_GENERAL_LIABILITY_SUBROGATION_WAIVER     = "tagSWGL";

        public const string S_AUTOMOBILE_INSURER_LETTER                 = "Chk7";
        public const string S_AUTOMOBILE_ANY_AUTO                       = "Chk8";
        public const string S_AUTOMOBILE_ALL_OWNED_AUTOS                = "Chk9";
        public const string S_AUTOMOBILE_SCHEDULED_OWNED_AUTOS          = "Chk10";
        public const string S_AUTOMOBILE_HIRED_OWNED_AUTOS              = "tagALHiredAutos";
        public const string S_AUTOMOBILE_NON_OWNED_AUTOS                = "tagALNonOwnedAutos";
        public const string S_AUTOMOBILE_OTHER1                         = "Chk13";
        public const string S_AUTOMOBILE_OTHER1_TEXT                    = "Auto1";
        public const string S_AUTOMOBILE_OTHER2                         = "Chk13a";
        public const string S_AUTOMOBILE_OTHER2_TEXT                    = "Auto2";
        public const string S_AUTOMOBILE_POLICY_NUMBER                  = "tagALPolicyNo";
        public const string S_AUTOMOBILE_EFFECTIVE_DATE                 = "tagALEffectiveDate";
        public const string S_AUTOMOBILE_EXPIRY_DATE                    = "tagALExpirationDate";
        public const string S_AUTOMOBILE_LIMIT_COMBINED                 = "tagALCombinedSingleLimit";
        public const string S_AUTOMOBILE_LIMIT_INJURY_PERSON            = "tagALBodilyInjuryPerson";
        public const string S_AUTOMOBILE_LIMIT_INJURY_ACCIDENT          = "tagALBodilyInjuryAccident";
        public const string S_AUTOMOBILE_LIMIT_PROPERTY_DAMAGE          = "tagALPropertyDamage";
        public const string S_AUTOMOBILE_LIMIT_OTHER_TEXT               = "AutoBlankLimitDesc";
        public const string S_AUTOMOBILE_LIMIT_OTHER                    = "AutoBlankLimitAmount";
        public const string S_AUTOMOBILE_ADDITIONAL_INSURED             = "tagAIAuto";
        public const string S_AUTOMOBILE_SUBROGATION_WAIVER             = "tagSWAuto";

        public const string S_UMBRELLA_INSURER_LETTER                   = "Chk17";
        public const string S_UMBRELLA_LIABILITY                        = "ChkUmb";
        public const string S_UMBRELLA_OCCURRENCE                       = "tagUmbrellaOccurrence";
        public const string S_UMBRELLA_EXCESS                           = "ChkExcess";
        public const string S_UMBRELLA_CLAIMS_MADE                      = "tagUmbrellaClaimsMade";
        public const string S_UMBRELLA_DED                              = "Chk18";
        public const string S_UMBRELLA_RETENTION                        = "Chk19";
        public const string S_UMBRELLA_LIMIT_RETENTION                  = "tagUmbrellaRetainedLimit";
        public const string S_UMBRELLA_POLICY_NUMBER                    = "tagUmbrellaPolicyNo";
        public const string S_UMBRELLA_EFFECTIVE_DATE                   = "tagumbrellaEffectiveDate";
        public const string S_UMBRELLA_EXPIRY_DATE                      = "tagUmbrellaExpirationDate";
        public const string S_UMBRELLA_LIMIT_OCCURRENCE                 = "tagUmbrellaLiabilityEachOccurrence";
        public const string S_UMBRELLA_LIMIT_AGGREGATE                  = "tagUmbrellaLiability";
        public const string S_UMBRELLA_LIMIT_OTHER_TEXT                 = "ExcessLine2Text";
        public const string S_UMBRELLA_LIMIT_OTHER                      = "ExcessLine2";
        public const string S_UMBRELLA_ADDITIONAL_INSURED               = "tagAIUmb";
        public const string S_UMBRELLA_SUBROGATION_WAIVER               = "tagSWUmb";

        public const string S_WORKER_INSURER_LETTER                     = "Chk20";
        public const string S_WORKER_EXCLUDED_PARTNER                   = "Excluded";
        public const string S_WORKER_POLICY_NUMBER                      = "tagELPolicyNo";
        public const string S_WORKER_EFEECTIVATE_DATE                   = "tagELEffectiveDate";
        public const string S_WORKER_EXPIRY_DATE                        = "tagELExpirationDate";
        public const string S_WORKER_STATUTORY                          = "tagWCStatutoryLimits";
        public const string S_WORKER_OTHER                              = "Chk22";
        public const string S_WORKER_LIMIT_ACCIDENT                     = "tagELEachAccident";
        public const string S_WORKER_LIMIT_EMPLOYEE                     = "tagELDiseaseEachEmployee";
        public const string S_WORKER_LIMIT_POLICY                       = "tagELDiseasePolicyLimit";
        public const string S_WORKER_SUBROGATION_WAIVER                 = "tagSWWC";

        public const string S_OTHER_INSURER_LETTER                      = "Chk23";
        public const string S_OTHER_TYPE                                = "OtherType";
        public const string S_OTHER_POLICY_NUMBER                       = "OtherPolicyNo";
        public const string S_OTHER_EFFECITVE_DATE                      = "Eff6";
        public const string S_OTHER_EXPIRY_DATE                         = "Exp6";
        public const string S_OTHER_LIMITS                              = "OtherLimits";
        public const string S_OTHER_ADDITIONAL_INSURED                  = "tagAIOther";
        public const string S_OTHER_SUBROGATION_WAIVER                  = "tagSWOther";

        public const string S_DESCRIPTION                               = "tagClientCertDescription";
        public const string S_DESCRIPTIONCOLUMN                         = "tagClientCertDescriptionCol";
        public const string S_HOLDER                                    = "tagCertHolderAll";
        public const string S_SIGNATURE                                 = "tagImgAuthorisedSignature";
    }
}