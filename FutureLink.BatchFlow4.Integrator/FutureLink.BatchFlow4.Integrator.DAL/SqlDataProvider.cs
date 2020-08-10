using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace FutureLink.BatchFlow4.Integrator.DAL
{
    public class SqlDataProvider : DataProvider
    {


        public override Info.CurrencyInfo GetCurrencyByCode(string connectionString, string code)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[p_GetCurrencyRate]", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Code", code));
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);
                    if (reader.Read())
                        return GetCurrencyInfoFromReader(reader);
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetConfigurationByKey", ex);
                return null;
            }
        }

        public override List<Info.CustomerInfo> GetCustomers(string connectionString)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_Get_All_Connections", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);
                    return GetCustomersFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetCustomers", ex);
                return new List<Info.CustomerInfo>();
            }
        }

        public override Info.ConfigurationInfo GetConfigurationByKey(string connectionString, string key)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_GetconfigurationByKey", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@keyname", key));
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);
                    if (reader.Read())
                        return GetConfigurationFromReader(reader);
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetConfigurationByKey", ex);
                return null;
            }

        }

        public override List<Info.SqlParamInfo> GetSqlSpParams(string connectionstring, string pName)
        {
            try
            {
                List<Info.SqlParamInfo> myParams = new List<Info.SqlParamInfo>();
                using (SqlConnection cn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand(pName, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    SqlCommandBuilder.DeriveParameters(cmd);
                    foreach (SqlParameter p in cmd.Parameters)
                    {
                        Info.SqlParamInfo myParam = new Info.SqlParamInfo();
                        if (p.Direction == System.Data.ParameterDirection.Input || p.Direction == System.Data.ParameterDirection.InputOutput)
                        {
                            myParam.ParamName = p.ParameterName;
                            myParam.ParamType = p.DbType;
                            myParam.ParamDirection = p.Direction;
                            myParams.Add(myParam);
                        }
                    }
                    cn.Close();
                    return myParams;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, "database", "GetSqlSpParams");
                return new List<Info.SqlParamInfo>();
            }
        }


        public override bool AddVendor(string connectionstring, Info.CreditorInfo vendor)
        {
            try
            {
                SqlParameter sqlParam = null;
                List<Info.SqlParamInfo> parameters = GetSqlSpParams(connectionstring, "p_Creditors_IntegrateV2");
                using (SqlConnection cn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("p_Creditors_IntegrateV2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    foreach (Info.SqlParamInfo param in parameters)
                    {
                        object value = GetParamValue(param, vendor.Fields);
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = param.ParamName;
                        sqlParam.Direction = param.ParamDirection;
                        sqlParam.Value = value;
                        cmd.Parameters.Add(sqlParam);
                    }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "AddVendor", ex);
                return false;
            }
        }



        public override bool AddDimension(string connectionstring, Info.DimensionInfo dimension)
        {
            try
            {
                SqlParameter sqlParam = null;
                List<Info.SqlParamInfo> parameters = GetSqlSpParams(connectionstring, "p_ImportDimensionScheduleV2");
                using (SqlConnection cn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("p_ImportDimensionScheduleV2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    foreach (Info.SqlParamInfo param in parameters)
                    {
                        object value = GetParamValue(param, dimension.Fields);
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = param.ParamName;
                        sqlParam.Direction = param.ParamDirection;
                        sqlParam.Value = value;
                        cmd.Parameters.Add(sqlParam);
                    }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "AddDimension", ex);
                return false;
            }
        }


        public override bool AddFinanceAccount(string connectionstring, Info.FinanceAccountInfo financeaccount)
        {
            try
            {
                SqlParameter sqlParam = null;
                List<Info.SqlParamInfo> parameters = GetSqlSpParams(connectionstring, "p_ImportAcchountScheduleV3");
                using (SqlConnection cn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("p_ImportAcchountScheduleV3", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    foreach (Info.SqlParamInfo param in parameters)
                    {
                        object value = GetParamValue(param, financeaccount.Fields);
                        sqlParam = new SqlParameter();
                        sqlParam.ParameterName = param.ParamName;
                        sqlParam.Direction = param.ParamDirection;
                        sqlParam.Value = value;
                        cmd.Parameters.Add(sqlParam);
                    }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "AddFinanceAccount", ex);
                return false;
            }
        }

        public override List<Info.DocumentInfo> GetDocumentsForExport(string connectionstring)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionstring))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand("p_FromIpuls_Accepted", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    cn.Open();
                    da.Fill(ds);
                    return GetDocumentsFromDataset(ds);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetCustomers", ex);
                return new List<Info.DocumentInfo>();
            }

        }

        public override bool SetKeyValue(string connectionString, string key, string value)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_SetKeyValue", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.Add(new SqlParameter("@KeyName", key));
                    cmd.Parameters.Add(new SqlParameter("@KeyValue", value));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "SetKeyValue", ex);
                return false;
            }
        }

        public override bool MarkDocAsExported(string connectionString, Info.PostingReturnValueInfo doc)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_Handle_Exported_Accepted_Doc", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.Add(new SqlParameter("@DocId", doc.DataId));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "MarkDocAsExported", ex);
                return false;
            }
        }

        public override bool UpdateVoucherNo(string connectionString, Info.PostingReturnValueInfo doc)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_ImportERPNumber", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.Add(new SqlParameter("@DocId", doc.DataId));
                    cmd.Parameters.Add(new SqlParameter("@ERPNumber", doc.VoucherNo));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "UpdateVoucherNo", ex);
                return false;
            }
        }

        public override bool MarkDocAsFailed(string connectionString, Info.PostingReturnValueInfo doc)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_ExportFailed", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.Add(new SqlParameter("@DocId", doc.DocumentId));
                    cmd.Parameters.Add(new SqlParameter("@ErrorText", doc.ErrorText));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "MarkDocAsFailed", ex);
                return false;
            }
        }


        private string GetFileInfo(string connectionString, Guid documentId)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    string Result = "";
                    SqlCommand cmd = new SqlCommand("p_GetDocument", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", documentId));
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);
                    if (reader.Read())
                    {
                        Result = reader["Filename"].ToString();
                    }
                    else
                        Result = "";
                    return Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetFileInfo", ex);
                return "";
            }

        }


        public override List<string> GetDocumentFiles(string connectionString, Guid DocumentId)
        {
            List<string> fileList = new List<string>();
            string Result = "";
            try
            {
                Result = GetFileInfo(connectionString, DocumentId);
                if (Result.Length > 0)
                    fileList.Add(Result);

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("p_Documents_GetFolder", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FolderId", DocumentId));
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                Result = "";
                                Guid fileId = new Guid(reader["Id"].ToString());
                                Result = GetFileInfo(connectionString, fileId);
                                if (Result.Length > 0)
                                    fileList.Add(Result);
                            }
                            catch (Exception ex)
                            {
                                _logger.Fatal(ex.Message, "SqlDataProvider", "GetDocumentFiles", ex);
                            }
                        }
                    }
                    return fileList;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "SqlDataProvider", "GetDocumentFiles", ex);
                return fileList;
            }
        }
    }
}

