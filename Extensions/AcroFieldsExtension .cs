﻿using System;
using BizCover.Common.Entities;
using BizCover.Common.Helpers;
using BizCover.Utility.Document.Template.Constants;
using iTextSharp.text.pdf;

namespace BizCover.Utility.Document.Template.Extensions
{
    public static class AcroFieldsExtention
    {
        public static bool SetField(this AcroFields fields, string fieldName, bool value)
        {
            return fields.SetField(fieldName, value ? "X" : "");
        }

        public static bool SetFieldYesNo(this AcroFields fields, string fieldName, bool value)
        {
            return fields.SetField(fieldName, value ? "Y" : "N");
        }

        public static bool SetField(this AcroFields fields, string fieldName, DateTime value)
        {
            return fields.SetField(fieldName, value.ToString(BCDateHelper.DateFormatShort));
        }

        public static bool SetField(this AcroFields fields, string fieldName, decimal value)
        {
            return fields.SetField(fieldName, value.ToString(FormatConstant.LimitAmount));
        }

        public static bool SetField(this AcroFields fields, string fieldName, BCPrice price)
        {
            if (price != null)
            {
                return fields.SetField(fieldName, price.StringValue);
            }

            return true;
        }
    }
}