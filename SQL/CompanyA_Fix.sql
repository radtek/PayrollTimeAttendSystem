
USE [InteractPayroll_00009]
GO

--INSERT INTO [dbo].[LEAVE]
--           ([COMPANY_NO]
--           ,[EMPLOYEE_NO]
--           ,[EARNING_NO]
--           ,[PAY_CATEGORY_TYPE]
--           ,[LEAVE_REC_NO]
--           ,[LEAVE_DESC]
--           ,[PROCESS_NO]
--           ,[LEAVE_PROCESSED_DATE]
--           ,[LEAVE_FROM_DATE]
--           ,[LEAVE_TO_DATE]
--           ,[LEAVE_ACCUM_DAYS]
--           ,[LEAVE_PAID_DAYS]
--           ,[LEAVE_OPTION]
--           ,[LEAVE_HOURS_DECIMAL]
--           ,[USER_NO_NEW_RECORD]
--           ,[DATETIME_NEW_RECORD])
   



--SELECT

-- E.COMPANY_NO
--           ,E.EMPLOYEE_NO
--           ,EN.EARNING_NO
--           ,E.PAY_CATEGORY_TYPE
--           ,1
--           ,'Take-On Balance'
--           ,99
--           ,'2012-08-25'
--           ,'2012-08-25'
--           ,'2012-08-25'
--           ,0
--           ,0
--           ,'D'
--           ,0
--           ,0
--           ,'2012-08-25'
--  FROM [InteractPayroll_00009].[dbo].[EMPLOYEE] E

--  INNER JOIN [InteractPayroll_00009].[dbo].[EARNING] EN
--   ON E.COMPANY_NO = EN.COMPANY_NO
--  AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE
--  AND EN.EARNING_NO IN (200,201)


--  LEFT JOIN [InteractPayroll_00009].[dbo].[LEAVE] L
--  ON E.COMPANY_NO = L.COMPANY_NO
--  AND E.EMPLOYEE_NO = L.EMPLOYEE_NO
--  AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE

--  WHERE E.PAY_CATEGORY_TYPE = 'W'
--  AND E.EMPLOYEE_NO < 118
--  AND L.COMPANY_NO IS NULL

--INSERT INTO [dbo].[LEAVE]
--           ([COMPANY_NO]
--           ,[EMPLOYEE_NO]
--           ,[EARNING_NO]
--           ,[PAY_CATEGORY_TYPE]
--           ,[LEAVE_REC_NO]
--           ,[LEAVE_DESC]
--           ,[PROCESS_NO]
--           ,[LEAVE_PROCESSED_DATE]
--           ,[LEAVE_FROM_DATE]
--           ,[LEAVE_TO_DATE]
--           ,[LEAVE_ACCUM_DAYS]
--           ,[LEAVE_PAID_DAYS]
--           ,[LEAVE_OPTION]
--           ,[LEAVE_HOURS_DECIMAL]
--           ,[USER_NO_NEW_RECORD]
--           ,[DATETIME_NEW_RECORD])
   



--SELECT

-- E.COMPANY_NO
--           ,E.EMPLOYEE_NO
--           ,EN.EARNING_NO
--           ,E.PAY_CATEGORY_TYPE
--           ,1
--           ,'Take-On Balance'
--           ,99
--           ,'2012-09-01'
--           ,'2012-09-01'
--           ,'2012-09-01'
--           ,0
--           ,0
--           ,'D'
--           ,0
--           ,0
--           ,'2012-09-01'
--  FROM [InteractPayroll_00009].[dbo].[EMPLOYEE] E

--  INNER JOIN [InteractPayroll_00009].[dbo].[EARNING] EN
--   ON E.COMPANY_NO = EN.COMPANY_NO
--  AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE
--  AND EN.EARNING_NO IN (200,201)


--  LEFT JOIN [InteractPayroll_00009].[dbo].[LEAVE] L
--  ON E.COMPANY_NO = L.COMPANY_NO
--  AND E.EMPLOYEE_NO = L.EMPLOYEE_NO
--  AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE

--  WHERE E.PAY_CATEGORY_TYPE = 'S'
--  AND E.EMPLOYEE_NO < 118
--  AND L.COMPANY_NO IS NULL


--INSERT INTO [dbo].[LEAVE]
--           ([COMPANY_NO]
--           ,[EMPLOYEE_NO]
--           ,[EARNING_NO]
--           ,[PAY_CATEGORY_TYPE]
--           ,[LEAVE_REC_NO]
--           ,[LEAVE_DESC]
--           ,[PROCESS_NO]
--           ,[LEAVE_PROCESSED_DATE]
--           ,[LEAVE_FROM_DATE]
--           ,[LEAVE_TO_DATE]
--           ,[LEAVE_ACCUM_DAYS]
--           ,[LEAVE_PAID_DAYS]
--           ,[LEAVE_OPTION]
--           ,[LEAVE_HOURS_DECIMAL]
--           ,[USER_NO_NEW_RECORD]
--           ,[DATETIME_NEW_RECORD])


--SELECT

-- E.COMPANY_NO
--           ,E.EMPLOYEE_NO
--           ,EN.EARNING_NO
--           ,E.PAY_CATEGORY_TYPE
--           ,1
--           ,'Take-On Balance'
--           ,99
--           ,E.EMPLOYEE_TAX_STARTDATE
--           ,E.EMPLOYEE_TAX_STARTDATE
--           ,E.EMPLOYEE_TAX_STARTDATE
--           ,0
--           ,0
--           ,'D'
--           ,0
--           ,0
--          ,E.EMPLOYEE_TAX_STARTDATE

--  FROM [InteractPayroll_00009].[dbo].[EMPLOYEE] E

--  INNER JOIN [InteractPayroll_00009].[dbo].[EARNING] EN
--   ON E.COMPANY_NO = EN.COMPANY_NO
--  AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE
--  AND EN.EARNING_NO IN (200,201)


--  LEFT JOIN [InteractPayroll_00009].[dbo].[LEAVE] L
--  ON E.COMPANY_NO = L.COMPANY_NO
--  AND E.EMPLOYEE_NO = L.EMPLOYEE_NO
--  AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE

--  WHERE  L.COMPANY_NO IS NULL

--UPDATE E

--SET E.EMPLOYEE_TAX_STARTDATE = L.LEAVE_PROCESSED_DATE
--,E.EMPLOYEE_LAST_RUNDATE = L.LEAVE_PROCESSED_DATE

--  FROM [InteractPayroll_00009].[dbo].[EMPLOYEE] E

 
--  INNER JOIN [InteractPayroll_00009].[dbo].[LEAVE] L
--  ON E.COMPANY_NO = L.COMPANY_NO
--  AND E.EMPLOYEE_NO = L.EMPLOYEE_NO
--  AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE

--  WHERE  E.EMPLOYEE_TAX_STARTDATE <> L.LEAVE_PROCESSED_DATE

--UPDATE EMPLOYEE_INFO_HISTORY
--SET EMPLOYEE_LAST_RUNDATE = NULL
--WHERE RUN_TYPE = 'T'

--DELETE FROM  EMPLOYEE_INFO_HISTORY
--WHERE RUN_TYPE = 'P'


--UPDATE EIH

--SET EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE

--FROM EMPLOYEE_INFO_HISTORY EIH




--INNER JOIN  [InteractPayroll_00009].[dbo].[EMPLOYEE] E
--ON EIH.COMPANY_NO = E.COMPANY_NO
--AND EIH.EMPLOYEE_NO = E.EMPLOYEE_NO
--AND EIH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE


--DELETE FROM EMPLOYEE_DEDUCTION_HISTORY
--WHERE RUN_TYPE = 'P'


--UPDATE EDH

--SET EDH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE

--FROM EMPLOYEE_DEDUCTION_HISTORY EDH




--INNER JOIN  [InteractPayroll_00009].[dbo].[EMPLOYEE] E
--ON EDH.COMPANY_NO = E.COMPANY_NO
--AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO
--AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE


--DELETE FROM EMPLOYEE_EARNING_HISTORY
--WHERE RUN_TYPE = 'P'


--UPDATE EDH

--SET EDH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE

--FROM EMPLOYEE_EARNING_HISTORY EDH




--INNER JOIN  [InteractPayroll_00009].[dbo].[EMPLOYEE] E
--ON EDH.COMPANY_NO = E.COMPANY_NO
--AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO
--AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE

--DELETE FROM EMPLOYEE_PAY_CATEGORY_HISTORY
--WHERE RUN_TYPE = 'P'



UPDATE EDH

SET EDH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE

FROM EMPLOYEE_PAY_CATEGORY_HISTORY EDH




INNER JOIN  [InteractPayroll_00009].[dbo].[EMPLOYEE] E
ON EDH.COMPANY_NO = E.COMPANY_NO
AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO
AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE