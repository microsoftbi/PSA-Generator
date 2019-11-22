﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator
{
    public class PSA_TYPE2
    {
        public static string GenerateTableSTG()
        {
            string result = "";

            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {
                sb.AppendLine("CREATE TABLE [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "]");
                sb.AppendLine("(");

                sb.AppendLine("\t[SEQUENCE_NO] [bigint] NULL,");
                sb.AppendLine("\t[SESSION_DTS] [datetimeoffset](7) NOT NULL,");
                sb.AppendLine("\t[Fully_Qualified_File_Name] [varchar](25) NOT NULL,");
                sb.AppendLine("\t[FILE_TRANSFER_DTS] [datetimeoffset](7) NOT NULL,");
                sb.AppendLine("\t[REC_SRC] [varchar](20) NULL,");


                //Fields
                var lstColumns = (from p in lstMetas where p.TABLE_NAME == itemTable.TABLE_NAME where p.PK == 1 || p.BK == 1 || p.DI == 1 select p).ToList();

                int n = lstColumns.Count;
                int pt = 0;

                foreach (var itemColumn in lstColumns)
                {
                    pt++;

                    if (pt < n)
                    {
                        sb.AppendLine(Common.FieldGenerate(itemColumn.COLUMN_NAME, itemColumn.DATA_TYPE, itemColumn.CHARACTER_MAXIMUM_LENGTH, itemColumn.NUMERIC_PRECISION, itemColumn.NUMERIC_SCALE) + ",");
                    }
                    else
                    {
                        sb.AppendLine(Common.FieldGenerate(itemColumn.COLUMN_NAME, itemColumn.DATA_TYPE, itemColumn.CHARACTER_MAXIMUM_LENGTH, itemColumn.NUMERIC_PRECISION, itemColumn.NUMERIC_SCALE));
                    }
                }

                sb.AppendLine(")");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();

            return result;
        }

        public static string GenerateTableCDC()
        {
            string result = "";

            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {
                sb.AppendLine("CREATE TABLE [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "]");
                sb.AppendLine("(");

                sb.AppendLine("\t[LOAD_DTS] [datetimeoffset](7) NOT NULL,");
                sb.AppendLine("\t[LOAD_DTS_BATCH] [datetimeoffset](7) NOT NULL,");
                sb.AppendLine("\t[SEQUENCE_NO] [bigint] NOT NULL,");
                sb.AppendLine("\t[CDC_OPERATION_CODE] [char](1) NOT NULL,");
                sb.AppendLine("\t[RECORD_SOURCE] [nvarchar](15) NOT NULL,");
                sb.AppendLine("\t[FULLY_QUALIFIED_FILE_NAME] [nvarchar](256) NULL,");
                sb.AppendLine("\t[INSERT_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[UPDATE_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[EXPORT_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[FILE_TRANSFER_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[SESSION_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[SOURCE_SLICE_DTS] [datetimeoffset](7) NULL,");
                sb.AppendLine("\t[LOAD_TYPE] [nvarchar](10) NULL,");
                sb.AppendLine("\t[HK] [char](32) NOT NULL,");
                sb.AppendLine("\t[HD] [char](32) NOT NULL,");
                sb.AppendLine("\t[HF] [char](32) NOT NULL,");



                //Fields
                var lstColumns = (from p in lstMetas where p.TABLE_NAME == itemTable.TABLE_NAME where p.PK == 1 || p.BK == 1 || p.DI == 1 select p).ToList();

                int n = lstColumns.Count;
                int pt = 0;

                foreach (var itemColumn in lstColumns)
                {
                    pt++;

                    if (pt < n)
                    {
                        sb.AppendLine(Common.FieldGenerate(itemColumn.COLUMN_NAME, itemColumn.DATA_TYPE, itemColumn.CHARACTER_MAXIMUM_LENGTH, itemColumn.NUMERIC_PRECISION, itemColumn.NUMERIC_SCALE) + ",");
                    }
                    else
                    {
                        sb.AppendLine(Common.FieldGenerate(itemColumn.COLUMN_NAME, itemColumn.DATA_TYPE, itemColumn.CHARACTER_MAXIMUM_LENGTH, itemColumn.NUMERIC_PRECISION, itemColumn.NUMERIC_SCALE));
                    }
                }

                sb.AppendLine(")");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();

            return result;
        }

        public static string GenerateTableLOG()
        {
            return GenerateTableCDC();
        }

        public static string GenerateVIEWMTA()
        {
            string result = "";

            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            string PK = "";

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {
                PK = (from p in dc.ATTRIBUTE where p.TABLE_NAME == itemTable.TABLE_NAME && p.PK==1 select p.COLUMN_NAME).ToList()[0];
                //Fields
                var lstColumns = (from p in lstMetas where p.TABLE_NAME == itemTable.TABLE_NAME where p.PK == 1 || p.BK == 1 || p.DI == 1 select p).ToList();

                sb.AppendLine("USE [STAGE]");
                sb.AppendLine("GO");
                sb.AppendLine("CREATE VIEW [" + itemTable.RecordSource + "].[V_" + itemTable.TABLE_NAME + "_MTA]");
                sb.AppendLine("AS");

                sb.AppendLine("\tSELECT CAST(NULL AS DATETIMEOFFSET(7)) AS LOAD_DTS,");
                sb.AppendLine("\t\tTODATETIMEOFFSET(");
                sb.AppendLine("\t\t\tCAST(CAST(CAST([FILE_TRANSFER_DTS] AS DATE) AS VARCHAR(10)) + ' '");
                sb.AppendLine("\t\t\t\t+ CAST(CAST('00:00:00' AS TIME) AS VARCHAR(16)) AS DATETIME2(7)),");
                sb.AppendLine("\t\t\tDATEDIFF(MINUTE, GETUTCDATE(), GETDATE())");
                sb.AppendLine("\t\t) AS LOAD_DTS_BATCH,");
                sb.AppendLine("\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\tCAST(NULL AS NCHAR(1)) AS CDC_OPERATION_CODE,");
                sb.AppendLine("\t\t[REC_SRC],");
                sb.AppendLine("\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\tCAST(NULL AS DATETIMEOFFSET(7)) AS [INSERT_DTS],");
                sb.AppendLine("\t\tCAST(NULL AS DATETIMEOFFSET(7)) AS [UPDATE_DTS],");
                sb.AppendLine("\t\tCAST(NULL AS DATETIMEOFFSET(7)) AS [EXPORT_DTS],");
                sb.AppendLine("\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t[SESSION_DTS],");
                sb.AppendLine("\t\tCAST(NULL AS DATETIMEOFFSET(7)) AS [SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\tCAST('FULL' AS NVARCHAR(10)) AS [LOAD_TYPE],");
                sb.AppendLine("\t\tCONVERT(CHAR(32), HASHBYTES('MD5', ISNULL(TRIM(CONVERT(NVARCHAR(50), [Dealer_Code])), N'') + N'W|D'), 2) AS HK,");
                sb.AppendLine("\t\t\tCONVERT(");
                sb.AppendLine("\t\t\t\tCHAR(32),");
                sb.AppendLine("\t\t\t\tHASHBYTES(");
                sb.AppendLine("\t\t\t\t\t'MD5',");

                int n = lstColumns.Count;
                int pt = 0;

                foreach (var itemColumn in lstColumns)
                {
                    pt++;

                    if (itemColumn.PK == 1) continue;

                    if (pt == 1)
                    {
                        sb.AppendLine("\t\t\t\t\tISNULL(TRIM(CONVERT(NVARCHAR(255), [" + itemColumn.COLUMN_NAME + "])), N'') + N'W|D'");
                    }
                    else
                    {
                        sb.AppendLine("\t\t\t\t\t+ ISNULL(TRIM(CONVERT(NVARCHAR(255), [" + itemColumn.COLUMN_NAME + "])), N'') + N'W|D'");
                    }
                }

                sb.AppendLine("\t\t\t\t),");
                sb.AppendLine("\t\t\t\t2");
                sb.AppendLine("\t\t\t\t) AS HD,");

                sb.AppendLine("\t\tCONVERT(CHAR(32), HASHBYTES('MD5', ISNULL(TRIM(CONVERT(NVARCHAR(50), [Dealer_Code])), N'') + N'W|D'), 2) AS HK,");
                sb.AppendLine("\t\t\tCONVERT(");
                sb.AppendLine("\t\t\t\tCHAR(32),");
                sb.AppendLine("\t\t\t\tHASHBYTES(");
                sb.AppendLine("\t\t\t\t\t'MD5',");

                //int n = lstColumns.Count;
                pt = 0;

                foreach (var itemColumn in lstColumns)
                {
                    pt++;

                    if (pt == 1)
                    {
                        sb.AppendLine("\t\t\t\t\tISNULL(TRIM(CONVERT(NVARCHAR(255), [" + itemColumn.COLUMN_NAME + "])), N'') + N'W|D'");
                    }
                    else
                    {
                        sb.AppendLine("\t\t\t\t\t+ ISNULL(TRIM(CONVERT(NVARCHAR(255), [" + itemColumn.COLUMN_NAME + "])), N'') + N'W|D'");
                    }
                }

                sb.AppendLine("\t\t\t\t),");
                sb.AppendLine("\t\t\t\t2");
                sb.AppendLine("\t\t\t\t) AS HF,");

                //DI fields
                foreach (var itemColumn in lstColumns)
                {
                    sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                }

                sb.AppendLine("\tFROM [IMP].[dbo].[imp_dealer_geo_data];");
                sb.AppendLine("GO");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();

            return result;
        }

        public static string GenerateVIECURRENT()
        {
            string result = "";


            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            string PK = "";

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {
                PK = (from p in dc.ATTRIBUTE where p.TABLE_NAME == itemTable.TABLE_NAME && p.PK == 1 select p.COLUMN_NAME).ToList()[0];
                //Fields
                var lstColumns = (from p in lstMetas where p.TABLE_NAME == itemTable.TABLE_NAME where p.PK == 1 || p.BK == 1 || p.DI == 1 select p).ToList();

                sb.AppendLine("USE [STAGE]");
                sb.AppendLine("GO");
                sb.AppendLine("CREATE VIEW [" + itemTable.RecordSource + "].[V_" + itemTable.TABLE_NAME + "_LOG_CURRENT]");
                sb.AppendLine("AS");

                sb.AppendLine("\tSELECT LogTable.[LOAD_DTS]");
                sb.AppendLine("\t\t,LogTable.[LOAD_DTS_BATCH]");
                sb.AppendLine("\t\t,LogTable.[SEQUENCE_NO]");
                sb.AppendLine("\t\t,LogTable.[CDC_OPERATION_CODE]");
                sb.AppendLine("\t\t,LogTable.[RECORD_SOURCE]");
                sb.AppendLine("\t\t,LogTable.[FULLY_QUALIFIED_FILE_NAME]");
                sb.AppendLine("\t\t,LogTable.[INSERT_DTS]");
                sb.AppendLine("\t\t,LogTable.[UPDATE_DTS]");
                sb.AppendLine("\t\t,LogTable.[EXPORT_DTS]");
                sb.AppendLine("\t\t,LogTable.[FILE_TRANSFER_DTS]");
                sb.AppendLine("\t\t,LogTable.[SESSION_DTS]");
                sb.AppendLine("\t\t,LogTable.[SOURCE_SLICE_DTS]");
                sb.AppendLine("\t\t,LogTable.[LOAD_TYPE]");
                sb.AppendLine("\t\t,LogTable.[HK]");
                sb.AppendLine("\t\t,LogTable.[HD]");
                sb.AppendLine("\t\t,LogTable.[HF]");

                //DI fields
                foreach (var itemColumn in lstColumns)
                {
                    sb.AppendLine("\t\t[" + itemColumn.COLUMN_NAME + "],");
                }

                sb.AppendLine("\tFROM [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG] AS LogTable");
                sb.AppendLine("\tINNER JOIN");
                sb.AppendLine("\t(");
                sb.AppendLine("\t\tSELECT HK,");
                sb.AppendLine("\t\t\tMAX(LOAD_DTS) AS MAX_LOAD_DTS");
                sb.AppendLine("\t\tFROM [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG] WITH (NOLOCK)");
                sb.AppendLine("\t\tGROUP BY HK");
                sb.AppendLine("\t) AS HDA_MAX");
                sb.AppendLine("\tON LogTable.HK = HDA_MAX.HK");
                sb.AppendLine("\t\tAND LogTable.LOAD_DTS = HDA_MAX.MAX_LOAD_DTS");
                sb.AppendLine("\tWHERE LogTable.CDC_OPERATION_CODE != 'D';");
                sb.AppendLine("GO");

                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();


            return result;
        }

        public static string GenerateUSPLOG()
        {
            string result = "";

            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {

                sb.AppendLine("USE [STAGE]");
                sb.AppendLine("GO");
                sb.AppendLine("CREATE PROCEDURE [" + itemTable.RecordSource + "].[USP_" + itemTable.TABLE_NAME + "_LOG]");
                sb.AppendLine("AS");
                sb.AppendLine("\tBEGIN");
                

                //Fields
                var lstColumns = (from metas in lstMetas where metas.TABLE_NAME == itemTable.TABLE_NAME where metas.PK == 1 || metas.BK == 1 || metas.DI == 1 select metas).ToList();
                int n = lstColumns.Count;
                int p = 0;

                sb.AppendLine("\tDECLARE @LOGSOURCE AS NVARCHAR(100);");
                sb.AppendLine("\tSET @LOGSOURCE = N'STAGE.dbo.USP_XXX';");
                sb.AppendLine("\t");
                sb.AppendLine("\tEXEC META.dbo.USP_WRITELOG N'Start to load XXX', @LOGSOURCE, N'N';");
                sb.AppendLine("\t");
                sb.AppendLine("\tBEGIN TRY");
                sb.AppendLine("\t\tBEGIN TRAN;");
                sb.AppendLine("\t");
                sb.AppendLine("\t\t\tINSERT INTO [dbo].[imp_Dealer_Geo_Data_Log]");
                sb.AppendLine("\t\t\t(");
                sb.AppendLine("\t\t\t\t[LOAD_DTS],");
                sb.AppendLine("\t\t\t\t[LOAD_DTS_BATCH],");
                sb.AppendLine("\t\t\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\t\t\t[CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\t[RECORD_SOURCE],");
                sb.AppendLine("\t\t\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\t[INSERT_DTS],");
                sb.AppendLine("\t\t\t\t[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\t[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\t[SESSION_DTS],");
                sb.AppendLine("\t\t\t\t[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\t[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\t[HK],");
                sb.AppendLine("\t\t\t\t[HD],");
                sb.AppendLine("\t\t\t\t[HF],");


                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p<n-1) 
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\t)");
                sb.AppendLine("\t\t\tSELECT CDC.LOAD_DTS,");
                sb.AppendLine("\t\t\t\tCDC.LOAD_DTS_BATCH,");
                sb.AppendLine("\t\t\t\tCDC.SEQUENCE_NO,");
                sb.AppendLine("\t\t\t\tCDC.CDC_OPERATION_CODE,");
                sb.AppendLine("\t\t\t\tCDC.RECORD_SOURCE,");
                sb.AppendLine("\t\t\t\tCDC.FULLY_QUALIFIED_FILE_NAME,");
                sb.AppendLine("\t\t\t\tCDC.INSERT_DTS,");
                sb.AppendLine("\t\t\t\tCDC.UPDATE_DTS,");
                sb.AppendLine("\t\t\t\tCDC.EXPORT_DTS,");
                sb.AppendLine("\t\t\t\tCDC.FILE_TRANSFER_DTS,");
                sb.AppendLine("\t\t\t\tCDC.SESSION_DTS,");
                sb.AppendLine("\t\t\t\tCDC.SOURCE_SLICE_DTS,");
                sb.AppendLine("\t\t\t\tCDC.LOAD_TYPEASLOAD_TYPE,");
                sb.AppendLine("\t\t\t\tCDC.HKASHK,");
                sb.AppendLine("\t\t\t\tCDC.HDASHD,");
                sb.AppendLine("\t\t\t\tCDC.HFASHF,");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\tFROM [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_CDC] AS CDC");
                sb.AppendLine("\t\t\tWHERE CDC.[HF] NOT IN");
                sb.AppendLine("\t\t\t(");
                sb.AppendLine("\t\t\t\tSELECT [HF] FROM [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG]");
                sb.AppendLine("\t\t\t  );");
                sb.AppendLine("\t\t\t");
                sb.AppendLine("\t\t\tEXEC META.dbo.USP_WRITELOG N'Finish to load " + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG', @LOGSOURCE, N'N';");
                sb.AppendLine("\t\t\t");
                sb.AppendLine("\t\tCOMMIT TRAN;");
                sb.AppendLine("\t");
                sb.AppendLine("\tBEGIN CATCH");
                sb.AppendLine("\t");
                sb.AppendLine("\t\tDECLARE @ERROR_MESSAGE AS NVARCHAR(4000);");
                sb.AppendLine("\t\tSET @ERROR_MESSAGE = N'Failed to load " + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG' + ISNULL(ERROR_MESSAGE(), '');");
                sb.AppendLine("\t\tEXEC META.dbo.USP_WRITELOG @ERROR_MESSAGE, @LOGSOURCE, N'E';");
                sb.AppendLine("\tEND CATCH");
                sb.AppendLine("\tEND;");
                sb.AppendLine("\tGO");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();

            return result;
        }

        public static string GenerateUSPCDC()
        {
            string result = "";

            DataClassesDataContext dc = new DataClassesDataContext();

            var lstMetas = (from p in dc.ATTRIBUTE select p).ToList();

            var lstTables = (from p in lstMetas select new { p.TABLE_NAME, p.RecordSource }).ToList();

            StringBuilder sb = new StringBuilder();

            //Table
            foreach (var itemTable in lstTables.Distinct())
            {

                sb.AppendLine("USE [STAGE]");
                sb.AppendLine("GO");
                sb.AppendLine("CREATE PROCEDURE [" + itemTable.RecordSource + "].[USP_" + itemTable.TABLE_NAME + "_CDC]");
                sb.AppendLine("AS");
                sb.AppendLine("\tBEGIN");


                //Fields
                var lstColumns = (from metas in lstMetas where metas.TABLE_NAME == itemTable.TABLE_NAME where metas.PK == 1 || metas.BK == 1 || metas.DI == 1 select metas).ToList();
                int n = lstColumns.Count;
                int p = 0;

                sb.AppendLine("\tDECLARE @LOGSOURCE AS NVARCHAR(100);");
                sb.AppendLine("\tSET @LOGSOURCE = N'STAGE.[" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_CDC]';");
                sb.AppendLine("\t");
                sb.AppendLine("\tEXEC META.dbo.USP_WRITELOG N'Start to load [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_CDC]', @LOGSOURCE, N'N';");
                sb.AppendLine("\t");
                sb.AppendLine("\tTRUNCATE TABLE [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_CDC];");
                sb.AppendLine("\t");
                sb.AppendLine("\tBEGIN TRY");
                sb.AppendLine("\t\tBEGIN TRAN;");
                sb.AppendLine("\t\t\tINSERT INTO [" + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_CDC]");
                sb.AppendLine("\t\t\t(");
                sb.AppendLine("\t\t\t\t[LOAD_DTS],");
                sb.AppendLine("\t\t\t\t[LOAD_DTS_BATCH],");
                sb.AppendLine("\t\t\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\t\t\t[CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\t[RECORD_SOURCE],");
                sb.AppendLine("\t\t\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\t[INSERT_DTS],");
                sb.AppendLine("\t\t\t\t[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\t[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\t[SESSION_DTS],");
                sb.AppendLine("\t\t\t\t[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\t[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\t[HK],");
                sb.AppendLine("\t\t\t\t[HD],");
                sb.AppendLine("\t\t\t\t[HF],");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\t)");
                sb.AppendLine("\t\t\tSELECT DATEADD(");
                sb.AppendLine("\t\t\t\tSECOND,");
                sb.AppendLine("\t\t\t\t((IIF(Mta.SOURCE_ENTITY = 'HDA', 0, Mta.SEQUENCE_NO) * 100) / 1000000000),");
                sb.AppendLine("\t\t\t\t DATEADD(");
                sb.AppendLine("\t\t\t\t\tNANOSECOND,");
                sb.AppendLine("\t\t\t\t\t((IIF(Mta.SOURCE_ENTITY = 'HDA', 0, Mta.SEQUENCE_NO)) * 100) % 1000000000,");
                sb.AppendLine("\t\t\t\t\tIIF(Mta.SOURCE_ENTITY = 'HDA',");
                sb.AppendLine("\t\t\t\t\t\t DATEADD(");
                sb.AppendLine("\t\t\t\t\t\t\tNS,");
                sb.AppendLine("\t\t\t\t\t\t\tROW_NUMBER() OVER (ORDER BY (SELECT 0)) * 100,");
                sb.AppendLine("\t\t\t\t\t\t\tMAX_LTS.MAX_LOAD_DTS_BATCH");
                sb.AppendLine("\t\t\t\t\t\t),");
                sb.AppendLine("\t\t\t\t\t Mta.LOAD_DTS_BATCH)");
                sb.AppendLine("\t\t\t\t\t )");
                sb.AppendLine("\t\t\t\t) LOAD_DTS,");
                sb.AppendLine("\t\t\t\tIIF(Mta.SOURCE_ENTITY = 'HDA', MAX_LTS.MAX_LOAD_DTS_BATCH, Mta.LOAD_DTS_BATCH) LOAD_DTS_BATCH,");
                sb.AppendLine("\t\t\t\tIIF(Mta.SOURCE_ENTITY = 'HDA', 0, Mta.SEQUENCE_NO) SEQUENCE_NO,");
                sb.AppendLine("\t\t\t\tCASE");
                sb.AppendLine("\t\t\t\t\tWHEN Mta.LEAD_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\t\tAND Mta.SOURCE_ENTITY = 'HDA'");
                sb.AppendLine("\t\t\t\t\t\t\tAND MAX_LTS.LOAD_TYPE = 'FULL' THEN");
                sb.AppendLine("\t\t\t\t'D'");
                sb.AppendLine("\t\t\t\t\tWHEN Mta.LAG_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\t\tAND Mta.SOURCE_ENTITY = 'CDC' THEN");
                sb.AppendLine("\t\t\t\t'I'");
                sb.AppendLine("\t\t\t\t\tWHEN Mta.LAG_HF <> HF");
                sb.AppendLine("\t\t\t\t\t\t\tAND NOT Mta.LAG_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\t\tAND Mta.SOURCE_ENTITY = 'CDC' THEN");
                sb.AppendLine("\t\t\t\t'U'");
                sb.AppendLine("\t\t\t\t\tELSE");
                sb.AppendLine("\t\t\t\t''");
                sb.AppendLine("\t\t\t\tEND [CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\tMta.[RECORD_SOURCE],");
                sb.AppendLine("\t\t\t\tMta.[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\tMta.[INSERT_DTS],");
                sb.AppendLine("\t\t\t\tMta.[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\tMta.[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\tMta.[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\tMta.[SESSION_DTS],");
                sb.AppendLine("\t\t\t\tMta.[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\tMta.[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\tMta.[HK],");
                sb.AppendLine("\t\t\t\tMta.[HD],");
                sb.AppendLine("\t\t\t\tMta.[HF],");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\tFROM");
                sb.AppendLine("\t\t\t(");
                sb.AppendLine("\t\t\t\tSELECT LAG(HF) OVER (PARTITION BY HK ORDER BY LOAD_DTS_BATCH, RNK, SEQUENCE_NO) AS LAG_HF,");
                sb.AppendLine("\t\t\t\t\tLEAD(HF) OVER (PARTITION BY HK ORDER BY LOAD_DTS_BATCH, RNK, SEQUENCE_NO) AS LEAD_HF,");
                sb.AppendLine("\t\t\t\t\t[LOAD_DTS],");
                sb.AppendLine("\t\t\t\t\t[LOAD_DTS_BATCH],");
                sb.AppendLine("\t\t\t\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\t\t\t\t[CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\t\t[RECORD_SOURCE],");
                sb.AppendLine("\t\t\t\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\t\t[INSERT_DTS],");
                sb.AppendLine("\t\t\t\t\t[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\t\t[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\t\t[SESSION_DTS],");
                sb.AppendLine("\t\t\t\t\t[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\t\t[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\t\t[HK],");
                sb.AppendLine("\t\t\t\t\t[HD],");
                sb.AppendLine("\t\t\t\t\t[HF],");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\t\t\t[SOURCE_ENTITY],");
                sb.AppendLine("\t\t\t\t\t[RNK]");
                sb.AppendLine("\t\t\t\tFROM");
                sb.AppendLine("\t\t\t\t(");
                sb.AppendLine("\t\t\t\t\tSELECT [LOAD_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[LOAD_DTS_BATCH],");
                sb.AppendLine("\t\t\t\t\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\t\t\t\t\t[CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\t\t\t[RECORD_SOURCE],");
                sb.AppendLine("\t\t\t\t\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\t\t\t[INSERT_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[SESSION_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\t\t\t[HK],");
                sb.AppendLine("\t\t\t\t\t\t[HD],");
                sb.AppendLine("\t\t\t\t\t\t[HF],");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\t\t\t\t'HDA' AS SOURCE_ENTITY,");
                sb.AppendLine("\t\t\t\t\t\t1 AS RNK");
                sb.AppendLine("\t\t\t\t\tFROM [" + itemTable.RecordSource + "].[V_" + itemTable.TABLE_NAME + "_LOG_CURRENT]");
                sb.AppendLine("\t\t\t\t\tUNION ALL");
                sb.AppendLine("\t\t\t\t\tSELECT [LOAD_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[LOAD_DTS_BATCH],");
                sb.AppendLine("\t\t\t\t\t\t[SEQUENCE_NO],");
                sb.AppendLine("\t\t\t\t\t\t[CDC_OPERATION_CODE],");
                sb.AppendLine("\t\t\t\t\t\t[REC_SRC],");
                sb.AppendLine("\t\t\t\t\t\t[FULLY_QUALIFIED_FILE_NAME],");
                sb.AppendLine("\t\t\t\t\t\t[INSERT_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[UPDATE_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[EXPORT_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[FILE_TRANSFER_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[SESSION_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[SOURCE_SLICE_DTS],");
                sb.AppendLine("\t\t\t\t\t\t[LOAD_TYPE],");
                sb.AppendLine("\t\t\t\t\t\t[HK],");
                sb.AppendLine("\t\t\t\t\t\t[HD],");
                sb.AppendLine("\t\t\t\t\t\t[HF],");

                p = 0;
                foreach (var itemColumn in lstColumns)
                {
                    p++;
                    if (p < n - 1)
                        sb.AppendLine("\t\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "],");
                    else
                        sb.AppendLine("\t\t\t\t\t\t[" + itemColumn.COLUMN_NAME + "]");
                }

                sb.AppendLine("\t\t\t\t\t\t'CDC' AS SOURCE_ENTITY,");
                sb.AppendLine("\t\t\t\t\t\t2 AS RNK");
                sb.AppendLine("\t\t\t\t\tFROM [" + itemTable.RecordSource + "].[V_"+itemTable.TABLE_NAME+"_MTA]");
                sb.AppendLine("\t\t\t\t) Mta");
                sb.AppendLine("\t\t\t) Mta");
                sb.AppendLine("\t\t\tCROSS JOIN");
                sb.AppendLine("\t\t\t(");
                sb.AppendLine("\t\t\t\tSELECT MAX(LOAD_DTS_BATCH) AS MAX_LOAD_DTS_BATCH,");
                sb.AppendLine("\t\t\t\t\t\tMAX(LOAD_TYPE) AS LOAD_TYPE");
                sb.AppendLine("\t\t\t\tFROM [" + itemTable.RecordSource + "].[V_" + itemTable.TABLE_NAME + "_MTA]");
                sb.AppendLine("\t\t\t) AS MAX_LTS");
                sb.AppendLine("\t\t\tWHERE 1 = 1");
                sb.AppendLine("\t\t\t\tAND CASE");
                sb.AppendLine("\t\t\t\t\tWHEN LEAD_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\tAND SOURCE_ENTITY = 'HDA'");
                sb.AppendLine("\t\t\t\t\t\tAND MAX_LTS.LOAD_TYPE = 'FULL' THEN");
                sb.AppendLine("\t\t\t\t\t\t'D' --kein Nachfolger für HDA-Datensatz in CDC");
                sb.AppendLine("\t\t\t\t\tWHEN LAG_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\tAND SOURCE_ENTITY = 'CDC' THEN");
                sb.AppendLine("\t\t\t\t\t\t'I' --kein Vorgänger für CDC-Datensatz in HDA");
                sb.AppendLine("\t\t\t\t\tWHEN LAG_HF <> HF");
                sb.AppendLine("\t\t\t\t\t\tAND NOT LAG_HF IS NULL");
                sb.AppendLine("\t\t\t\t\t\tAND SOURCE_ENTITY = 'CDC' THEN");
                sb.AppendLine("\t\t\t\t\t\t'U'");
                sb.AppendLine("\t\t\t\t\tELSE");
                sb.AppendLine("\t\t\t\t\t\t''");
                sb.AppendLine("\t\t\t\tEND <> '';");

                sb.AppendLine("\t\t\tEXEC META.dbo.USP_WRITELOG N'Finish to load " + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG', @LOGSOURCE, N'N';");

                sb.AppendLine("\t\tCOMMIT TRAN;");
                sb.AppendLine("\tEND TRY");
                sb.AppendLine("\tBEGIN CATCH");
                sb.AppendLine("\t\tROLLBACK TRAN;");
                sb.AppendLine("\t\tDECLARE @ERROR_MESSAGE AS NVARCHAR(4000);");
                sb.AppendLine("\t\tSET @ERROR_MESSAGE = N'Failed to load " + itemTable.RecordSource + "].[" + itemTable.TABLE_NAME + "_LOG' + ISNULL(ERROR_MESSAGE(), '');");
                sb.AppendLine("\t\tEXEC META.dbo.USP_WRITELOG @ERROR_MESSAGE, @LOGSOURCE, N'E';");
                sb.AppendLine("\tEND CATCH");
                sb.AppendLine("END;");
                sb.AppendLine("GO");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            result = sb.ToString();

            return result;
        }
    }
}
