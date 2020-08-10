using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace FutureLink.BatchFlow4.Integrator.DAL
{
    public abstract class DataProvider
    {
        protected Info.LoggingClass _logger = new Info.LoggingClass("FutureLink.BatchFlow4.Integrator.DAL");
        private static DataProvider _provider = null;

        public static DataProvider Instance()
        {
            if (_provider == null)
                _provider = (DataProvider)new SqlDataProvider();
            return _provider;
        }


        public abstract Info.CurrencyInfo GetCurrencyByCode(string connectionString, string code);
        public abstract List<Info.CustomerInfo> GetCustomers(string connectionString);
        public abstract Info.ConfigurationInfo GetConfigurationByKey(string connectionString, string key);
        public abstract bool SetKeyValue(string connectionString, string key, string value);
        public abstract List<Info.SqlParamInfo> GetSqlSpParams(string connectionstring, string pName);
        public abstract bool AddVendor(string connectionstring, Info.CreditorInfo vendor);
        public abstract bool AddDimension(string connectionstring, Info.DimensionInfo dimension);
        public abstract bool AddFinanceAccount(string connectionstring, Info.FinanceAccountInfo financeaccount);
        public abstract List<Info.DocumentInfo> GetDocumentsForExport(string connectionstring);
        public abstract bool MarkDocAsExported(string connectionString, Info.PostingReturnValueInfo doc);
        public abstract bool UpdateVoucherNo(string connectionString, Info.PostingReturnValueInfo doc);
        public abstract bool MarkDocAsFailed(string connectionString, Info.PostingReturnValueInfo doc);
        public abstract List<string> GetDocumentFiles(string connectionString, Guid DocumentId);


        protected Info.CurrencyInfo GetCurrencyInfoFromReader(IDataReader reader)
        {
            try
            {
                Info.CurrencyInfo curr = new Info.CurrencyInfo();
                curr.Code = reader["Code"].ToString();
                curr.Rate = Convert.ToDecimal(reader["ExchangeRate"]);
                return curr;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "DataProvider", "GetCurrencyInfoFromReader", ex);
                return null;
            }
        }

        protected Info.ConfigurationInfo GetConfigurationFromReader(IDataReader reader)
        {
            try
            {
                Info.ConfigurationInfo conf = new Info.ConfigurationInfo();
                conf.Id = new Guid(reader["id"].ToString());
                conf.Key = reader["Keyname"].ToString();
                conf.Value = reader["KeyValue"].ToString();
                conf.UserEditable = Convert.ToInt16(reader["UserEditAble"]);
                return conf;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "DataProvider", "GetConfigurationFromReader", ex);
                return null;
            }
        }


        protected Info.CustomerInfo GetCustomerFromReader(IDataReader reader)
        {
            try
            {
                Info.CustomerInfo cust = new Info.CustomerInfo();
                cust.Id = new Guid(reader["id"].ToString());
                cust.Number = Convert.ToInt64(reader["CustomerNumber"]);
                cust.Name = reader["CustomerName"].ToString();
                cust.Serial = reader["SerialNumber"].ToString();
                cust.ConnectionString = reader["ConnectionString"].ToString();
                cust.Active = Convert.ToBoolean(reader["Active"]);
                cust.DisplayName = reader["DisplayName"].ToString();
                cust.DbName = reader["dbName"].ToString();
                cust.UseEconomic = Convert.ToBoolean(reader["UseEconomic"]);
                return cust;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "DataProvider", "GetCustomerFromReader", ex);
                return null;
            }
        }

        protected List<Info.CustomerInfo> GetCustomersFromReader(IDataReader reader)
        {
            try
            {
                List<Info.CustomerInfo> custList = new List<Info.CustomerInfo>();
                while (reader.Read())
                {
                    Info.CustomerInfo cust = GetCustomerFromReader(reader);
                    if (cust != null)
                        custList.Add(cust);
                }
                return custList;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "DataProvider", "GetCustomersFromReader", ex);
                return new List<Info.CustomerInfo>();
            }
        }

        protected object GetParamValue(Info.SqlParamInfo param, List<Info.FieldInfo> fields)
        {
            foreach (Info.FieldInfo fi in fields)
            {
                if (fi.FieldName.ToLower() == param.ParamName.ToLower().Replace("@", ""))
                {
                    if (fi.FieldValue == null)
                        return GetDefaultValue(param.ParamType);
                    else
                        if (fi.FieldValue == DBNull.Value)
                        return GetDefaultValue(param.ParamType);
                    else
                        return fi.FieldValue;
                }
            }
            return GetDefaultValue(param.ParamType);
        }

        protected object GetDefaultValue(DbType paramType)
        {
            switch (paramType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "";
                case DbType.Boolean:
                    return false;
                case DbType.Byte:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.Single:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                    return 0;
                case DbType.DateTime:
                case DbType.DateTime2:
                    return DBNull.Value;
                default:
                    return "";
            }
        }

        protected Info.AccountingInfo GetAccountingInfoFromDataRow(DataRow dr)
        {
            Info.AccountingInfo ai = new Info.AccountingInfo();
            ai.AccountNo = dr["KontoNr"].ToString();
            if (dr["Amount"] == DBNull.Value || dr["Amount"] == null)
            {
                ai.Amount = 0;
                ai.AmountDefaultCurrency = 0;
            }
            else
            {
                ai.Amount = Convert.ToDecimal(dr["Amount"]);
                ai.AmountDefaultCurrency = Convert.ToDecimal(dr["Amount"]);
            }
            ai.PosteringsTekst = dr["PosteringsTekst"].ToString();
            ai.Department = dr["dim2"].ToString();
            ai.Project = dr["dim3"].ToString();
            ai.CostType = dr["dim1"].ToString();
            if (dr.Table.Columns.Contains("dim4"))
            {
                ai.Employee = dr["dim4"].ToString();
            }
            if (dr.Table.Columns.Contains("VatCode"))
                ai.VatCode = dr["VatCode"].ToString();
            else
                ai.VatCode = "";
            return ai;
        }

        protected Info.DocumentInfo GetDocumentInfoFromDataRow(DataRow dr)
        {
            try
            {
                Info.DocumentInfo di = new Info.DocumentInfo();
                di.DocumentId = new Guid(dr["id"].ToString());
                di.DataId = Convert.ToInt64(dr["docid"]);
                if (dr["Forfalds_Dato"] != DBNull.Value)
                    di.DueDate = Convert.ToDateTime(dr["Forfalds_Dato"]);
                else
                    di.DueDate = DateTime.Today;
                if (dr["InvoiceDate"] != DBNull.Value)
                    di.InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]);
                else
                    di.InvoiceDate = DateTime.Today;
                di.Amount = Convert.ToDecimal(dr["Belob"]);
                di.AmountDefaultCurrency = Convert.ToDecimal(dr["Belob"]);
                if (dr["InvoiceNo"] != DBNull.Value)
                    di.InvoiceNo = dr["InvoiceNo"].ToString();
                else
                    di.InvoiceNo = "";
                if (dr["KreditorNo"] != DBNull.Value)
                    di.VendorNo = dr["KreditorNo"].ToString();
                else
                    di.VendorNo = "";
                if (dr["Kreditor"] != DBNull.Value)
                    di.VendorName = dr["Kreditor"].ToString();
                else
                    di.VendorName = "";
                if (dr["CurrencyCode"] != DBNull.Value)
                    di.Currency = dr["CurrencyCode"].ToString();
                else
                    di.Currency = "";
                di.IsCreditnote = Convert.ToBoolean(dr["IsCreditnote"]);
                di.PaymentId = dr["PaymentReference"].ToString();
                if (dr.Table.Columns.Contains("Paymenttype"))
                {
                    if (dr["Paymenttype"] != DBNull.Value)
                        di.PaymentType = dr["Paymenttype"].ToString();
                    else
                        di.PaymentType = "";
                }
                if (dr.Table.Columns.Contains("Bankaccount"))
                {
                    if (dr["Bankaccount"] != DBNull.Value)
                        di.BankAccount = dr["Bankaccount"].ToString();
                    else
                        di.BankAccount = "";
                }
                if (dr.Table.Columns.Contains("PaymentCreditor"))
                {
                    if (dr["PaymentCreditor"] != DBNull.Value)
                        di.PaymentCreditorId = dr["PaymentCreditor"].ToString();
                    else
                        di.PaymentCreditorId = "";
                }
                if (dr.Table.Columns.Contains("Beholdningskonto"))
                {
                    if (dr["Beholdningskonto"] != DBNull.Value)
                        di.InventoryAccount = dr["Beholdningskonto"].ToString();
                    else
                        di.InventoryAccount = "";
                }


                return di;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "DataProvider", "GetDocumentInfoFromDataRow", ex);
                return null;
            }
        }


        protected List<Info.DocumentInfo> GetDocumentsFromDataset(DataSet ds)
        {
            List<Info.DocumentInfo> docList = new List<Info.DocumentInfo>();
            DataRelation drel = ds.Relations.Add("DocAcc", ds.Tables[0].Columns["id"], ds.Tables[1].Columns["itemid"]);
            foreach (DataRow docRow in ds.Tables[0].Rows)
            {
                Info.DocumentInfo doc = GetDocumentInfoFromDataRow(docRow);
                if (doc != null)
                {
                    List<Info.AccountingInfo> accLines = new List<Info.AccountingInfo>();
                    foreach (DataRow accRow in docRow.GetChildRows(drel))
                    {
                        Info.AccountingInfo accLine = GetAccountingInfoFromDataRow(accRow);
                        if (accLine != null)
                            accLines.Add(accLine);
                    }
                    doc.AccountingLines = accLines;
                    docList.Add(doc);
                }
            }
            return docList;
        }

    }
}
