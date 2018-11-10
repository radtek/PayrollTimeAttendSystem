using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Diagnostics;
using DPUruNet;

namespace FingerPrintClockServerLib
{
    public class clsFingerPrintClockServerLib
    {
        private string pvtstrSoftwareToUse = "";

        private int pvtintHeight = 381;
        private int pvtintWidth = 352;

        public clsFingerPrintClockServerLib(string SoftwareToUse)
        {
           pvtstrSoftwareToUse = SoftwareToUse;
        }

        public int Get_DP_Template(ref DataSet DataSet, int TemplateNo,byte[] bytPreviousFingerTemplate, byte[] bytCurrentFingeTemplate, ref byte[] bytEnrollmentFingerTemplate)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd(bytPreviousFingerTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
            DPUruNet.Fmd fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

            //Compare Template
            CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

            if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
            {
                int PROBABILITY_ONE = 0x7fffffff;
                int intFarRequested = PROBABILITY_ONE / 10000;

                if (cmrCompareResult.Score < intFarRequested) 
                {
                    if (TemplateNo == 4)
                    {
                        List<Fmd> preEnrollmentFmds = new List<Fmd>();

                        //Add Current Template
                        preEnrollmentFmds.Add(fmdCurrentFingerTemplate);

                        for (int intRow = 0; intRow < DataSet.Tables["Template"].Rows.Count; intRow++)
                        {
                            DPUruNet.Fmd fmdFingerTemplate = DPUruNet.Importer.ImportFmd((byte[])DataSet.Tables["Template"].Rows[intRow]["FINGER_TEMPLATE"], Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

                            preEnrollmentFmds.Add(fmdFingerTemplate);
                        }

                        DataResult<Fmd> drResultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, preEnrollmentFmds);

                        if (drResultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                        {
                            bytEnrollmentFingerTemplate = drResultEnrollment.Data.Bytes;
                        }
                        else
                        {
                            intReturnCode = 2;
                        }
                    }
                    else
                    {
                        bytEnrollmentFingerTemplate = bytCurrentFingeTemplate;
                    }
                }
                else
                {
                    intReturnCode = 1;
                }
            }
            else
            {
                intReturnCode = 1;
            }

            return intReturnCode;
        }
       
        //public int Get_Template(DataTable DataTable, int TemplateNo, int FingerPrintThreshold, ref int VerifyScoreFingerprintsCompare, ref byte[] bytGreyScaleRawImage, ref byte[] byteArrayPreviousTemplate, ref DPFP.Sample DPSample, ref byte[] bytExtractedTemplate, ref byte[] bytDPExtractedFeatures)
        //{
        //    int intReturnCode = 0;

        //    if (pvtstrSoftwareToUse == "D")
        //    {
        //        //NB Extraction Process if For Enrollment
        //        DPFP.Processing.FeatureExtraction DPExtractor = new DPFP.Processing.FeatureExtraction();
        //        DPFP.Capture.CaptureFeedback DPFeedback = DPFP.Capture.CaptureFeedback.None;
        //        DPFP.FeatureSet DPEnrollFeatures = new DPFP.FeatureSet();

        //        //Needs To Be Here - Falls Over If Very Bad Image
        //        try
        //        {
        //            DPExtractor.CreateFeatureSet(DPSample, DPFP.Processing.DataPurpose.Enrollment, ref DPFeedback, ref DPEnrollFeatures);
        //        }
        //        catch
        //        {
        //        }

        //        if (DPFeedback == DPFP.Capture.CaptureFeedback.Good)
        //        {
        //            if (TemplateNo == 1)
        //            {
        //                //Extract Template For Compare to Next Figure
        //                DPFP.Processing.Enrollment DPEnroller = new DPFP.Processing.Enrollment();

        //                for (int intCount = 1; intCount < 5; intCount++)
        //                {
        //                    DPEnroller.AddFeatures(DPEnrollFeatures);
        //                }

        //                if (DPEnroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
        //                {
        //                    bytExtractedTemplate = DPEnroller.Template.Bytes;
        //                }
        //                else
        //                {
        //                    //Bad Extraction
        //                    intReturnCode = 1;
        //                    goto Get_Template_Continue;
        //                }
        //            }

        //            bytDPExtractedFeatures = DPEnrollFeatures.Bytes;
        //        }
        //        else
        //        {
        //            //Bad Extraction
        //            intReturnCode = 1;
        //            goto Get_Template_Continue;
        //        }

        //        if (TemplateNo != 1)
        //        {
        //            //No Errors From Digital Persona
        //            if (intReturnCode == 0)
        //            {
        //                //Compare Template
        //                System.IO.MemoryStream stmStream = new MemoryStream(byteArrayPreviousTemplate);
        //                DPFP.Template DPTemplate = new DPFP.Template(stmStream);
        //                stmStream.Close();
        //                stmStream.Dispose();

        //                //Extract Features For Compare To Previous Template
        //                DPExtractor = null;
        //                DPExtractor = new DPFP.Processing.FeatureExtraction();
        //                DPFeedback = DPFP.Capture.CaptureFeedback.None;
        //                DPFP.FeatureSet DPVerificationFeatures = new DPFP.FeatureSet();

        //                //Needs To Be Here - Falls Over If Very Bad Image
        //                try
        //                {
        //                    DPExtractor.CreateFeatureSet(DPSample, DPFP.Processing.DataPurpose.Verification, ref DPFeedback, ref DPVerificationFeatures);
        //                }
        //                catch
        //                {
        //                }

        //                if (DPFeedback == DPFP.Capture.CaptureFeedback.Good)
        //                {
        //                    DPFP.Verification.Verification DPVerificator = new DPFP.Verification.Verification(FingerPrintThreshold);
        //                    DPFP.Verification.Verification.Result DPResult = new DPFP.Verification.Verification.Result();

        //                    DPVerificator.Verify(DPVerificationFeatures, DPTemplate, ref DPResult);

        //                    if (DPResult.Verified == true)
        //                    {
        //                        if (TemplateNo == 4)
        //                        {
        //                            DPFP.Processing.Enrollment DPEnroller = new DPFP.Processing.Enrollment();

        //                            DPEnroller.AddFeatures(DPEnrollFeatures);

        //                            for (int intRow = 0; intRow < DataTable.Rows.Count; intRow++)
        //                            {
        //                                stmStream = null;
        //                                stmStream = new MemoryStream((byte[])DataTable.Rows[intRow]["FINGER_FEATURES"]);
        //                                DPEnrollFeatures = null;
        //                                DPEnrollFeatures = new DPFP.FeatureSet(stmStream);

        //                                try
        //                                {
        //                                    DPEnroller.AddFeatures(DPEnrollFeatures);
        //                                }
        //                                catch
        //                                {
        //                                    break;
        //                                }
        //                            }

        //                            if (DPEnroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
        //                            {
        //                                bytExtractedTemplate = DPEnroller.Template.Bytes;
        //                            }
        //                            else
        //                            {
        //                                intReturnCode = 2;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        intReturnCode = 1;
        //                        goto Get_Template_Continue;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            VerifyScoreFingerprintsCompare = 0;
        //        }
        //    }

        //Get_Template_Continue:

        //    return intReturnCode;
        //}

        public int Get_Employee_Features(int FARRequested, DataTable EmployeeDataTable, ref byte[] bytCurrentFingeTemplate, ref int Row)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

            for (int intRow = 0; intRow < EmployeeDataTable.Rows.Count; intRow++)
            {
                if (EmployeeDataTable.Rows[intRow]["FINGER_TEMPLATE"] != System.DBNull.Value)
                {
                    DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd((byte[])EmployeeDataTable.Rows[intRow]["FINGER_TEMPLATE"], Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

                    //Compare Template
                    CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

                    if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        if (cmrCompareResult.Score < FARRequested)
                        {
                            Row = intRow;

                            break;
                        }
                    }
                }
            }

            return intReturnCode;
        }

        public int Get_Employee(DataTable EmployeeDataTable, ref byte[] bytCurrentFingeTemplate, int FingerPrintScoreThreshold, ref int FingerPrintScore, ref int Row)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

            for (int intRow = 0; intRow < EmployeeDataTable.Rows.Count; intRow++)
            {
                if (EmployeeDataTable.Rows[intRow]["FINGER_TEMPLATE"] != System.DBNull.Value)
                {
                    DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd((byte[])EmployeeDataTable.Rows[intRow]["FINGER_TEMPLATE"], Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

                    //Compare Template
                    CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

                    if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        int PROBABILITY_ONE = 0x7fffffff;
                        int intFarRequested = PROBABILITY_ONE / 5000;

                        if (cmrCompareResult.Score < intFarRequested)
                        {
                            Row = intRow;

                            break;
                        }
                    }
                }
            }

            return intReturnCode;
        }

        public int Get_User(DataTable UserDataTable, ref byte[] bytCurrentFingeTemplate, int FingerPrintScoreThreshold, ref int FingerPrintScore, ref int Row)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

            int PROBABILITY_ONE = 0x7fffffff;
            int intFarRequested = PROBABILITY_ONE / 5000;

            for (int intRow = 0; intRow < UserDataTable.Rows.Count; intRow++)
            {
                if (UserDataTable.Rows[intRow]["FINGER_TEMPLATE"] != System.DBNull.Value)
                {
                    DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd((byte[])UserDataTable.Rows[intRow]["FINGER_TEMPLATE"], Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

                    //Compare Template
                    CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

                    if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        if (cmrCompareResult.Score < intFarRequested)
                        {
                            Row = intRow;

                            break;
                        }
                    }
                }
            }

            return intReturnCode;
        }

        public int Get_User_Features(DataTable UserDataTable, ref byte[] bytCurrentFingeTemplate, ref int Row)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdCurrentFingerTemplate;

            if (bytCurrentFingeTemplate.Length == 101376)
            {
                //Curve - Embedded Linux
                fmdCurrentFingerTemplate = DPUruNet.FeatureExtraction.CreateFmdFromRaw(bytCurrentFingeTemplate, 0, 0, 288, 352, 500, Constants.Formats.Fmd.ANSI).Data; 
            }
            else
            {
                fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
            }

            int PROBABILITY_ONE = 0x7fffffff;
            int intFarRequested = PROBABILITY_ONE / 5000;

            for (int intRow = 0; intRow < UserDataTable.Rows.Count; intRow++)
            {
                if (UserDataTable.Rows[intRow]["FINGER_TEMPLATE"] != System.DBNull.Value)
                {
                    DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd((byte[])UserDataTable.Rows[intRow]["FINGER_TEMPLATE"], Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

                    //Compare Template
                    CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

                    if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        if (cmrCompareResult.Score < intFarRequested)
                        {
                            Row = intRow;

                            break;
                        }
                    }
                }
            }

            return intReturnCode;
        }
    }
}
