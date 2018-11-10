using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using IBscanUltimate;
using DPUruNet;

namespace InteractPayrollClient
{
    public partial class iUareSDK : UserControl
    {
        public delegate void OnCapturedEventHandler(CaptureResult captureResult);

        ColorPalette greyScalePalette;
        
        private int SIZE_HEIGHT = 0;
        private int SIZE_IMAGE = 0;
        private int SIZE_WIDTH = 0;

        //Curve Details
        private int CURVE_SIZE_HEIGHT = 352;
        private int CURVE_SIZE_IMAGE = 101376;
        private int CURVE_SIZE_WIDTH = 288;

        //Columbo Details
        private int COLUMBO_SIZE_HEIGHT = 500;
        private int COLUMBO_SIZE_IMAGE = 200000;
        private int COLUMBO_SIZE_WIDTH = 400;

        private int intCropX = 0;
        private int intCropY = 0;
        private int intCropHeight = 0;
        private int intCropWidth = 0;

        //URU5160 Details
        private const int URU_SIZE_HEIGHT = 392;
        private const int URU_SIZE_WIDTH = 357;
        private const int URU_SIZE_IMAGE = 139944;

        private string pvtstrDeviceDesc = "";

        public event OnCapturedEventHandler OnCaptured;

        private event OnCapturedEventHandler _OnCapturedSaved;
               
        private int m_nDevHandle = -1;

        private DateTime NextFlashDateTimeChange;
       
        private uint pvtUIntCaptureOptions = 0;
       
        private PictureBox myPictureBox;
      
        private bool pvtblnBlueColour = true;
        private bool pvtblnCaptureInProgress = false;

        private int pvtintMilliSeconds = 500;
        private bool pvtblnNeedClearPlaten = true;
        private bool pvtblnCurrentImageBlank = false;

        //Width(357) * Height(392) + Header(50)  = 139944 + 50 (Holds Raw Imahge)
        byte[] pvtbyteArrayFiv = new byte[139994];
        byte[] pvtCurveColumboByteArrayRGBBytes;
     
        private DLL.IBSU_CallbackResultImageEx m_callbackResultImageEx = null;
        private DLL.IBSU_CallbackClearPlatenAtCapture m_callbackClearPlaten = null;
        private DLL.IBSU_CallbackPreviewImage m_callbackPreviewImage = null;
        
        public enum ColourLED
        {
            Green = 0,
            Red = 1,
            Blue = 2,
            None = 3,
        }

        public iUareSDK(PictureBox PictureBox, OnCapturedEventHandler _OnCaptured)
        {
            InitializeComponent();
            
            if (PictureBox != null)
            {
                myPictureBox = PictureBox;
            }

            //2017-06-06 Create Greyscale Palette
            Bitmap bmpPalette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);

            greyScalePalette = bmpPalette.Palette;
            for (int i = 0; i < bmpPalette.Palette.Entries.Length; i++)
            {
                greyScalePalette.Entries[i] = Color.FromArgb(i, i, i);
            }

            //Save for Unhook when Device Closed
            _OnCapturedSaved = _OnCaptured;

            OnCaptured -= new OnCapturedEventHandler(_OnCapturedSaved);
            OnCaptured += new OnCapturedEventHandler(_OnCapturedSaved);
            
            pvtbyteArrayFiv = Enumerable.Repeat((byte)255, pvtbyteArrayFiv.Length).ToArray();
            
            //Fiv Header Creation
            pvtbyteArrayFiv[0] = 70;
            pvtbyteArrayFiv[1] = 73;
            pvtbyteArrayFiv[2] = 82;
            pvtbyteArrayFiv[3] = 0;
            pvtbyteArrayFiv[4] = 48;
            pvtbyteArrayFiv[5] = 49;
            pvtbyteArrayFiv[6] = 48;
            pvtbyteArrayFiv[7] = 0;
            pvtbyteArrayFiv[8] = 0;
            pvtbyteArrayFiv[9] = 0;
            pvtbyteArrayFiv[10] = 0;
            pvtbyteArrayFiv[11] = 2;
            pvtbyteArrayFiv[12] = 34;
            pvtbyteArrayFiv[13] = 218;
            pvtbyteArrayFiv[14] = 0;
            pvtbyteArrayFiv[15] = 51;
            pvtbyteArrayFiv[16] = 254;
            pvtbyteArrayFiv[17] = 255;
            pvtbyteArrayFiv[18] = 0;
            pvtbyteArrayFiv[19] = 0;
            pvtbyteArrayFiv[20] = 0;
            pvtbyteArrayFiv[21] = 30;
            pvtbyteArrayFiv[22] = 1;
            pvtbyteArrayFiv[23] = 1;
            pvtbyteArrayFiv[24] = 1;
            pvtbyteArrayFiv[25] = 244;
            pvtbyteArrayFiv[26] = 1;
            pvtbyteArrayFiv[27] = 244;
            pvtbyteArrayFiv[28] = 1;
            pvtbyteArrayFiv[29] = 244;
            pvtbyteArrayFiv[30] = 1;
            pvtbyteArrayFiv[31] = 244;
            pvtbyteArrayFiv[32] = 8;
            pvtbyteArrayFiv[33] = 0;
            pvtbyteArrayFiv[34] = 0;
            pvtbyteArrayFiv[35] = 0;
            pvtbyteArrayFiv[36] = 0;

            //Width * Height + Header = 357 * 392 + 14 = 139958

            //139958 = 131072 + 8704 + 182
            //131072 = (2*65536)
            pvtbyteArrayFiv[37] = 2;
            //8704 = (34 * 256)
            pvtbyteArrayFiv[38] = 34;
            //182 = (182 * 1)
            pvtbyteArrayFiv[39] = 182;

            pvtbyteArrayFiv[40] = 0;
            pvtbyteArrayFiv[41] = 1;
            pvtbyteArrayFiv[42] = 1;
            pvtbyteArrayFiv[43] = 75;

            pvtbyteArrayFiv[44] = 0;

            //Width = 357 = (1*256) + 101
            pvtbyteArrayFiv[45] = 1;
            pvtbyteArrayFiv[46] = 101;
            //Height = 392 = (1*256) + 96  
            pvtbyteArrayFiv[47] = 1;
            pvtbyteArrayFiv[48] = 136;

            pvtbyteArrayFiv[49] = 0;
        }

        public int OpenDevice()
        {
            int intReturnCode = 1;
            bool blnReaderFound = false;

            DLL.IBSU_DeviceDesc devDesc = new DLL.IBSU_DeviceDesc();

            int intNumberDevices = -1;

            intReturnCode = DLL._IBSU_GetDeviceCount(ref intNumberDevices);

            if (intNumberDevices > 0)
            {
                for (int intArrayNo = 0; intArrayNo < intNumberDevices; intArrayNo++)
                {
                    intReturnCode = DLL._IBSU_GetDeviceDescription(intArrayNo, ref devDesc);

                    if (intReturnCode == DLL.IBSU_STATUS_OK)
                    {
                        if (devDesc.productName == "CURVE"
                        || devDesc.productName == "COLUMBO")
                        {
                            blnReaderFound = true;

                            pvtstrDeviceDesc = devDesc.productName;

                            if (devDesc.productName == "CURVE")
                            {
                                intCropX = 10;
                                intCropY = 0;
                                intCropHeight = 275;
                                intCropWidth = 260;

                                SIZE_HEIGHT = CURVE_SIZE_HEIGHT;
                                SIZE_WIDTH = CURVE_SIZE_WIDTH;
                                SIZE_IMAGE = CURVE_SIZE_IMAGE;
                            }
                            else
                            {
                                intCropX = 21;
                                intCropY = 54;
                                intCropHeight = 392;
                                intCropWidth = 357;

                                SIZE_HEIGHT = COLUMBO_SIZE_HEIGHT;
                                SIZE_WIDTH = COLUMBO_SIZE_WIDTH;
                                SIZE_IMAGE = COLUMBO_SIZE_IMAGE;
                            }
                            
                            if (pvtCurveColumboByteArrayRGBBytes == null)
                            {
                                pvtCurveColumboByteArrayRGBBytes = new byte[SIZE_IMAGE];
                            }

                            int intDeviceHandle = -1;

                            intReturnCode = DLL._IBSU_OpenDevice(intArrayNo, ref intDeviceHandle);

                            if (intReturnCode == DLL.IBSU_STATUS_OK)
                            {
                                m_nDevHandle = intDeviceHandle;

                                m_callbackResultImageEx = new DLL.IBSU_CallbackResultImageEx(OnEvent_ResultImageEx);
                                m_callbackClearPlaten = new DLL.IBSU_CallbackClearPlatenAtCapture(OnEvent_ClearPlatenAtCapture);
                                m_callbackPreviewImage = new DLL.IBSU_CallbackPreviewImage(OnEvent_PreviewImage);

                                DLL._IBSU_RegisterCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_RESULT_IMAGE_EX, m_callbackResultImageEx, this.Handle);
                                DLL._IBSU_RegisterCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_OPTIONAL_EVENT_CLEAR_PLATEN_AT_CAPTURE, m_callbackClearPlaten, this.Handle);
                                DLL._IBSU_RegisterCallbacks(m_nDevHandle, DLL.IBSU_Events.ENUM_IBSU_ESSENTIAL_EVENT_PREVIEW_IMAGE, m_callbackPreviewImage, this.Handle);

                                //Set Capture Options
                                pvtUIntCaptureOptions |= DLL.IBSU_OPTION_AUTO_CONTRAST;
                                pvtUIntCaptureOptions |= DLL.IBSU_OPTION_AUTO_CAPTURE;
                            }
                            
                            //Only Cater for 1 Curve Reader pre Machine
                            break;
                        }
                        else
                        {
                            string message;
                            switch (intReturnCode)
                            {
                                case DLL.IBSU_ERR_DEVICE_ACTIVE:
                                    message = String.Format("[Error code = {0}] Device initialization failed because in use by another thread/process.", intReturnCode);
                               
                                    break;
                                case DLL.IBSU_ERR_USB20_REQUIRED:
                                    message = String.Format("[Error code = {0}] Device initialization failed because SDK only works with USB 2.0.", intReturnCode);
                                    break;
                                default:
                                    message = String.Format("[Error code = {0}] Device initialization failed", intReturnCode);
                                    break;
                            }
                        }
                    }
                }

                if (intReturnCode == 0)
                {
                    if (blnReaderFound == false)
                    {
                        intReturnCode = -1;
                    }
                }
            }
            else
            {
                intReturnCode = -201;
            }

            return intReturnCode;
        }

        public int CloseDevice()
        {
            if (this.OnCaptured != null)
            {
                OnCaptured -= new OnCapturedEventHandler(_OnCapturedSaved);
            }

            int intReturnCode = 0;

            if (pvtblnCaptureInProgress == true)
            {
                pvtblnCaptureInProgress = false;
                intReturnCode = StopCapture();
            }

            if (intReturnCode == 0)
            {
                bool blnActive = true;

                while (true)
                {
                    intReturnCode = DLL._IBSU_IsCaptureActive(m_nDevHandle, ref blnActive);

                    if (blnActive == false)
                    {
                        break;
                    }
                }

                intReturnCode = DLL._IBSU_CloseDevice(m_nDevHandle);
            }

            return intReturnCode;
        }

        public int StartCapture()
        {
            pvtblnCaptureInProgress = true;
            pvtblnNeedClearPlaten = true;
                   
            pvtblnCurrentImageBlank = false;
            
            //To fix Bug in IBScanUltimateSDK layer
            Thread.Sleep(100);

            int intReturnCode = DLL._IBSU_BeginCaptureImage(m_nDevHandle, DLL.IBSU_ImageType.ENUM_IBSU_FLAT_SINGLE_FINGER, DLL.IBSU_ImageResolution.ENUM_IBSU_IMAGE_RESOLUTION_500, pvtUIntCaptureOptions);

            if (intReturnCode == 0)
            {
                //InteractMiddleWare1 Section
                //NB THis Section Must Be After A StartCapture otherwise it Will NOT Work
                while (pvtblnNeedClearPlaten == true)
                {
                    Application.DoEvents();
                }
            }
            else
            {
                string stop = "";

            }

            return intReturnCode;
        }

        public int StopCapture()
        {
            int intReturnCode = DLL._IBSU_CancelCaptureImage(m_nDevHandle);

            pvtblnNeedClearPlaten = true;
            pvtblnCaptureInProgress = false;
            
            return intReturnCode;
        }

        public int SetLED(ColourLED parColourLED)
        {
            int intReturnCode = -1;

            if (parColourLED == ColourLED.Green)
            {
                intReturnCode = DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_SCAN_CURVE_GREEN);
            }
            else
            {
                if (parColourLED == ColourLED.Red)
                {
                    intReturnCode = DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_SCAN_CURVE_RED);
                }
                else
                {
                    if (parColourLED == ColourLED.Blue)
                    {
                        intReturnCode = DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_SCAN_CURVE_BLUE);
                    }
                    else
                    {
                        if (parColourLED == ColourLED.None)
                        {
                            intReturnCode = DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_NONE);
                        }
                    }
                }
            }

            return intReturnCode;
        }
        
        private delegate void OnEvent_ResultImageExThreadSafe(int deviceHandle, IntPtr pContext, int imageStatus, DLL.IBSU_ImageData image, DLL.IBSU_ImageType imageType, int detectedFingerCount, int segmentImageArrayCount, IntPtr pSegmentImageArray, IntPtr pSegmentPositionArray);
        
        private void OnEvent_ResultImageEx(int deviceHandle, IntPtr pContext, int imageStatus, DLL.IBSU_ImageData image, DLL.IBSU_ImageType imageType, int detectedFingerCount, int segmentImageArrayCount, IntPtr pSegmentImageArray, IntPtr pSegmentPositionArray)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new OnEvent_ResultImageExThreadSafe(OnEvent_ResultImageEx), new object[] { deviceHandle, pContext, imageStatus, image, imageType, detectedFingerCount, segmentImageArrayCount, pSegmentImageArray, pSegmentPositionArray });
            }
            else
            {
                if (pContext == null)
                {
                    return;
                }
                else
                {
                    try
                    {
                        pvtblnNeedClearPlaten = true;
                        pvtblnCaptureInProgress = false;

                        DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_NONE);

                        if (myPictureBox != null)
                        {
                            if (pvtstrDeviceDesc == "COLUMBO")
                            {
                                //NB pvtbyteArrayFiv is Created in Write_Columbo_Image  
                                Write_Columbo_Image(image.Buffer,true);
                            }
                            else
                            {
                                //NB pvtbyteArrayFiv is Created in Write_Curve_Image  
                                Write_Curve_Image(image.Buffer, true);
                            }
                                                       
                            DPUruNet.Fid myFid = DPUruNet.Importer.ImportFid(pvtbyteArrayFiv, Constants.Formats.Fid.ANSI).Data;

                            CaptureResult CaptureResultNew = new CaptureResult(Constants.ResultCode.DP_SUCCESS, Constants.CaptureQuality.DP_QUALITY_GOOD, 0, myFid);

                            OnCaptured(CaptureResultNew);
                                                      
                            myFid = null;
                            CaptureResultNew = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("OnEvent_ResultImage ex = " + ex.Message);
                    }
                }
            }
        }
        
        private delegate void OnEvent_ClearPlatenAtCaptureThreadSafe(int deviceIndex, IntPtr pContext, DLL.IBSU_PlatenState platenState);
        private void OnEvent_ClearPlatenAtCapture(int deviceIndex, IntPtr pContext, DLL.IBSU_PlatenState platenState)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new OnEvent_ClearPlatenAtCaptureThreadSafe(OnEvent_ClearPlatenAtCapture), new object[] { deviceIndex, pContext, platenState});
            }
            else
            {
                try
                {
                    if (pContext == null)
                    {
                        return;
                    }
                    else
                    {
                        if (platenState == DLL.IBSU_PlatenState.ENUM_IBSU_PLATEN_CLEARD)
                        {
                            pvtblnNeedClearPlaten = false;

                            pvtblnBlueColour = true;
                            DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_SCAN_CURVE_BLUE);

                            NextFlashDateTimeChange = DateTime.Now.AddMilliseconds(pvtintMilliSeconds);
                        }

                        GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("OnEvent_ClearPlatenAtCapture ex = " + ex.Message);
                }
            }
        }

        private void Write_Columbo_Picture_Image()
        {
            if (myPictureBox.Image != null)
            {
                //Otherwise memory Leak
                myPictureBox.Image.Dispose();
                myPictureBox.Image = null;
            }

            //Bitmap bmp = new Bitmap(Convert.ToInt32(image.Width), Convert.ToInt32(image.Height), PixelFormat.Format8bppIndexed);
            Bitmap bmp = new Bitmap(COLUMBO_SIZE_WIDTH, COLUMBO_SIZE_HEIGHT, PixelFormat.Format8bppIndexed);

            //Set Palette to GreyScale
            bmp.Palette = greyScalePalette;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //Copy From Safe Byte Array to BitmapData
            IntPtr p = new IntPtr(data.Scan0.ToInt64());
            System.Runtime.InteropServices.Marshal.Copy(pvtCurveColumboByteArrayRGBBytes, 0, p, COLUMBO_SIZE_IMAGE);

            bmp.UnlockBits(data);

            //Remove White Space From Around Image
            Rectangle recRectangle = new Rectangle(intCropX, intCropY, intCropWidth, intCropHeight);

            Bitmap bmpCrop = Bitmap.FromHbitmap(bmp.GetHbitmap()).Clone(recRectangle, PixelFormat.Format8bppIndexed);

            this.myPictureBox.Image = Image.FromHbitmap(bmpCrop.GetHbitmap());
            this.myPictureBox.Refresh();

            bmp.Dispose();
            bmp = null;
            bmpCrop.Dispose();
            bmpCrop = null;
            
            System.GC.Collect();
        }

        private void Write_Columbo_Image(IntPtr intPtrbuffer,bool isFinalImage)
        {
            System.Runtime.InteropServices.Marshal.Copy(intPtrbuffer, pvtCurveColumboByteArrayRGBBytes, 0, COLUMBO_SIZE_IMAGE);
            
            if (IsFingerTouching() == true)
            {
                Write_Columbo_Picture_Image();

                pvtblnCurrentImageBlank = false;
            }
            else
            {
                if (pvtblnCurrentImageBlank == false)
                {
                    //Make sure Picture is completely White
                    pvtCurveColumboByteArrayRGBBytes = Enumerable.Repeat((byte)255, pvtCurveColumboByteArrayRGBBytes.Length).ToArray();

                    Write_Columbo_Picture_Image();

                    pvtblnCurrentImageBlank = true;
                }
            }
            
            if (isFinalImage == true)
            {
                int intByteOffset = 50;
                //54 Rows, 400 Width
                //imagePointer += 40 * 357;
                int intColumboOffset = (54 * COLUMBO_SIZE_WIDTH);

                for (int i = 0; i < URU_SIZE_HEIGHT; i++)
                {
                    //Bytes In
                    intColumboOffset += 21;

                    for (int j = 0; j < URU_SIZE_WIDTH; j++)
                    {
                        pvtbyteArrayFiv[intByteOffset] = pvtCurveColumboByteArrayRGBBytes[intColumboOffset];

                        intByteOffset += 1;
                        intColumboOffset += 1;
                    }

                    //Bytes In and Bytes End Total = 400 - (21 + 357)
                    intColumboOffset += 22;
                }
            }

            System.GC.Collect();
        }

        bool IsFingerTouching()
        {
            bool blnFingerTouching = false;
            int intHeightIndex = 0;
            int intIndex = 0;
            int intWidthFrom = 48;
            int intWidthTo = SIZE_WIDTH - 48;

            for (int i = 48; i < SIZE_HEIGHT; i++)
            {
                intHeightIndex = i * SIZE_WIDTH;

                for (int j = intWidthFrom; j < intWidthTo; j++)
                {
                    intIndex = intHeightIndex + j;

                    //Close to Black Pixel
                    if (pvtCurveColumboByteArrayRGBBytes[intIndex] < 100)
                    {
                        blnFingerTouching = true;
                        break;
                    }
                }

                if (blnFingerTouching == true)
                {
                    break;
                }
                else
                {
                    i += 48;
                }
            }

            return blnFingerTouching;
        }

        private void Write_Curve_Picture_Image()
        {
            if (myPictureBox.Image != null)
            {
                //Otherwise memory Leak
                myPictureBox.Image.Dispose();
                myPictureBox.Image = null;
            }

            Bitmap bmp = new Bitmap(SIZE_WIDTH, SIZE_HEIGHT, PixelFormat.Format8bppIndexed);

            //Set Palette to GreyScale
            bmp.Palette = greyScalePalette;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //Copy From Safe Byte Array to BitmapData
            IntPtr p = new IntPtr(data.Scan0.ToInt64());
            System.Runtime.InteropServices.Marshal.Copy(pvtCurveColumboByteArrayRGBBytes, 0, p, SIZE_IMAGE);

            bmp.UnlockBits(data);

            //Remove White Space From Around Image
            Rectangle recRectangle = new Rectangle(intCropX, intCropY, intCropWidth, intCropHeight);

            Bitmap bmpCrop = Bitmap.FromHbitmap(bmp.GetHbitmap()).Clone(recRectangle, PixelFormat.Format8bppIndexed);

            this.myPictureBox.Image = Image.FromHbitmap(bmpCrop.GetHbitmap());
            this.myPictureBox.Refresh();

            bmp.Dispose();
            bmp = null;
            bmpCrop.Dispose();
            bmpCrop = null;
        }

        private void Write_Curve_Image(IntPtr intPtrbuffer, bool isFinalImage)
        {
            System.Runtime.InteropServices.Marshal.Copy(intPtrbuffer, pvtCurveColumboByteArrayRGBBytes, 0, SIZE_IMAGE);
            
            if (IsFingerTouching() == true)
            {
                Write_Curve_Picture_Image();

                pvtblnCurrentImageBlank = false;
            }
            else
            {
                if (pvtblnCurrentImageBlank == false)
                {
                    //Make sure Picture is completely White
                    pvtCurveColumboByteArrayRGBBytes = Enumerable.Repeat((byte)255, pvtCurveColumboByteArrayRGBBytes.Length).ToArray();

                    Write_Curve_Picture_Image();

                    pvtblnCurrentImageBlank = true;
                }
            }

            if (isFinalImage == true)
            {
                int intByteOffset = 50;
                int intCurveArrayOffset = 0;

                //40 Rows, 357 Width
                //imagePointer += 40 * 357;
                intByteOffset += 40 * URU_SIZE_WIDTH;

                for (int i = 0; i < CURVE_SIZE_HEIGHT; i++)
                {
                    //Bytes In
                    intByteOffset += 25;

                    for (int j = 0; j < CURVE_SIZE_WIDTH; j++)
                    {
                        pvtbyteArrayFiv[intByteOffset] = pvtCurveColumboByteArrayRGBBytes[intCurveArrayOffset];

                        intByteOffset += 1;
                        intCurveArrayOffset += 1;
                    }

                    //Bytes In and Bytes End Total = 69
                    intByteOffset += 44;
                }
            }
        }
        
        private delegate void OnEvent_PreviewImageThreadSafe(int deviceHandle, IntPtr pContext, DLL.IBSU_ImageData image);
        private void OnEvent_PreviewImage(
           int deviceHandle,
           IntPtr pContext,
           DLL.IBSU_ImageData image
           )
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new OnEvent_PreviewImageThreadSafe(OnEvent_PreviewImage), new object[] { deviceHandle, pContext, image});
            }
            else
            {
                if (pContext == null)
                {
                }
                else
                {
                    try
                    {
                        if (pvtblnNeedClearPlaten == false)
                        {
                            if (myPictureBox != null)
                            {
                                if (pvtstrDeviceDesc == "COLUMBO")
                                {
                                    Write_Columbo_Image(image.Buffer,false);
                                }
                                else
                                {
                                    Write_Curve_Image(image.Buffer, false);
                                }
                            }

                            if (NextFlashDateTimeChange < DateTime.Now)
                            {
                                if (pvtblnBlueColour == true)
                                {
                                    DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_NONE);

                                    pvtblnBlueColour = false;
                                }
                                else
                                {
                                    DLL._IBSU_SetLEDs(m_nDevHandle, DLL.IBSU_LED_SCAN_CURVE_BLUE);

                                    pvtblnBlueColour = true;
                                }

                                NextFlashDateTimeChange = DateTime.Now.AddMilliseconds(pvtintMilliSeconds);
                            }

                            GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("OnEvent_PreviewImage ex = " + ex.Message);
                    }
                }
            }
        }
    }
}
