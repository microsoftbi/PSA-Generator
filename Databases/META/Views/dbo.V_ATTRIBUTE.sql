SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO










CREATE VIEW [dbo].[V_ATTRIBUTE]
AS
SELECT ATT.[ID],
       ATT.[TABLE_CATALOG],
       ATT.[TABLE_NAME],
       ATT.[COLUMN_NAME],
       ATT.[DATA_TYPE],
       ATT.[CHARACTER_MAXIMUM_LENGTH],
       ATT.[NUMERIC_PRECISION],
       ATT.[NUMERIC_SCALE],
       ATTL.[DATA_TYPE] AS L_DATA_TYPE,
       ATTL.[CHARACTER_MAXIMUM_LENGTH] AS [L_CHARACTER_MAXIMUM_LENGTH],
       ATTL.[NUMERIC_PRECISION] AS [L_NUMERIC_PRECISION],
       ATTL.[NUMERIC_SCALE] AS [L_NUMERIC_SCALE],
       RS.RecordSourceName AS RECORDSOURCE,
       [BK],
       [PK],
       [DI],
	   [FK],
       --,[DVID]
       SAT.TableName AS DV_SAT_TABLENAME,
       HUB.TableName AS DV_HUB_TABLENAME,
	   LINK.TableName AS DV_LINK_TABLENAME,
       --HUB.BKNAMEAS AS DV_HUB_BK,
	   ATT.DV_COLUMN_NAME,
	   IS_FULLLOAD
FROM [META].[dbo].[ATTRIBUTE] ATT
    LEFT JOIN [META].[dbo].[ATTRIBUTE_LANDING] ATTL
        ON ATT.TABLE_CATALOG = ATTL.[TABLE_CATALOG]
           AND ATT.TABLE_NAME = ATTL.TABLE_NAME
           AND ATT.[COLUMN_NAME] = ATTL.[COLUMN_NAME]
    LEFT JOIN META.dbo.RecordSource RS
        ON ATT.TABLE_CATALOG = RS.DatabaseName
    LEFT JOIN META.dbo.DV_SAT SAT
        ON ATT.DV_SAT_ID = SAT.ID
    LEFT JOIN META.dbo.DV_HUB HUB
        ON ATT.DV_HUB_ID = HUB.ID
	LEFT JOIN META.dbo.DV_LINK LINK
		ON ATT.DV_LINK_ID= LINK.ID
	INNER JOIN [META].[dbo].GEN_LIST GEN
		ON ATT.[TABLE_CATALOG] + ATT.[TABLE_NAME]=GEN.[TABLE_CATALOG] + GEN.[TABLE_NAME] --AND IS_GEN=1
WHERE IS_GEN = 1;
GO
