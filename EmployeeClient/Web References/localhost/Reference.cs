﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace InteractPayrollClient.localhost {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="FingerPrintWebServiceSoap", Namespace="http://tempuri.org/")]
    public partial class FingerPrintWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback Get_Template_And_ImageOperationCompleted;
        
        private System.Threading.SendOrPostCallback Show_New_ImageOperationCompleted;
        
        private System.Threading.SendOrPostCallback Get_User_And_ImageOperationCompleted;
        
        private System.Threading.SendOrPostCallback Find_EmployeeOperationCompleted;
        
        private System.Threading.SendOrPostCallback Find_Employee_NamesOperationCompleted;
        
        private System.Threading.SendOrPostCallback Ping_DatabaseOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public FingerPrintWebService() {
            this.Url = global::InteractPayrollClient.Properties.Settings.Default.Employee_localhost_FingerPrintWebService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event Get_Template_And_ImageCompletedEventHandler Get_Template_And_ImageCompleted;
        
        /// <remarks/>
        public event Show_New_ImageCompletedEventHandler Show_New_ImageCompleted;
        
        /// <remarks/>
        public event Get_User_And_ImageCompletedEventHandler Get_User_And_ImageCompleted;
        
        /// <remarks/>
        public event Find_EmployeeCompletedEventHandler Find_EmployeeCompleted;
        
        /// <remarks/>
        public event Find_Employee_NamesCompletedEventHandler Find_Employee_NamesCompleted;
        
        /// <remarks/>
        public event Ping_DatabaseCompletedEventHandler Ping_DatabaseCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Get_Template_And_Image", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Get_Template_And_Image([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] ref byte[] bytPreviousTemplate, ref int FingerPrintScore, ref int VerifyScoreFingerprintsCompare, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] ref byte[] bytFingerImageByteArray) {
            object[] results = this.Invoke("Get_Template_And_Image", new object[] {
                        bytRawImageArray,
                        ShowBiometrics,
                        ShowMinutaiePoints,
                        bytPreviousTemplate,
                        FingerPrintScore,
                        VerifyScoreFingerprintsCompare,
                        bytFingerImageByteArray});
            bytPreviousTemplate = ((byte[])(results[0]));
            FingerPrintScore = ((int)(results[1]));
            VerifyScoreFingerprintsCompare = ((int)(results[2]));
            bytFingerImageByteArray = ((byte[])(results[3]));
        }
        
        /// <remarks/>
        public void Get_Template_And_ImageAsync(byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, byte[] bytPreviousTemplate, int FingerPrintScore, int VerifyScoreFingerprintsCompare, byte[] bytFingerImageByteArray) {
            this.Get_Template_And_ImageAsync(bytRawImageArray, ShowBiometrics, ShowMinutaiePoints, bytPreviousTemplate, FingerPrintScore, VerifyScoreFingerprintsCompare, bytFingerImageByteArray, null);
        }
        
        /// <remarks/>
        public void Get_Template_And_ImageAsync(byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, byte[] bytPreviousTemplate, int FingerPrintScore, int VerifyScoreFingerprintsCompare, byte[] bytFingerImageByteArray, object userState) {
            if ((this.Get_Template_And_ImageOperationCompleted == null)) {
                this.Get_Template_And_ImageOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGet_Template_And_ImageOperationCompleted);
            }
            this.InvokeAsync("Get_Template_And_Image", new object[] {
                        bytRawImageArray,
                        ShowBiometrics,
                        ShowMinutaiePoints,
                        bytPreviousTemplate,
                        FingerPrintScore,
                        VerifyScoreFingerprintsCompare,
                        bytFingerImageByteArray}, this.Get_Template_And_ImageOperationCompleted, userState);
        }
        
        private void OnGet_Template_And_ImageOperationCompleted(object arg) {
            if ((this.Get_Template_And_ImageCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Get_Template_And_ImageCompleted(this, new Get_Template_And_ImageCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Show_New_Image", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Show_New_Image([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytPreviousTemplate, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] ref byte[] bytFingerImageByteArray) {
            object[] results = this.Invoke("Show_New_Image", new object[] {
                        bytRawImageArray,
                        ShowBiometrics,
                        ShowMinutaiePoints,
                        bytPreviousTemplate,
                        bytFingerImageByteArray});
            bytFingerImageByteArray = ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void Show_New_ImageAsync(byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, byte[] bytPreviousTemplate, byte[] bytFingerImageByteArray) {
            this.Show_New_ImageAsync(bytRawImageArray, ShowBiometrics, ShowMinutaiePoints, bytPreviousTemplate, bytFingerImageByteArray, null);
        }
        
        /// <remarks/>
        public void Show_New_ImageAsync(byte[] bytRawImageArray, bool ShowBiometrics, bool ShowMinutaiePoints, byte[] bytPreviousTemplate, byte[] bytFingerImageByteArray, object userState) {
            if ((this.Show_New_ImageOperationCompleted == null)) {
                this.Show_New_ImageOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShow_New_ImageOperationCompleted);
            }
            this.InvokeAsync("Show_New_Image", new object[] {
                        bytRawImageArray,
                        ShowBiometrics,
                        ShowMinutaiePoints,
                        bytPreviousTemplate,
                        bytFingerImageByteArray}, this.Show_New_ImageOperationCompleted, userState);
        }
        
        private void OnShow_New_ImageOperationCompleted(object arg) {
            if ((this.Show_New_ImageCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Show_New_ImageCompleted(this, new Show_New_ImageCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Get_User_And_Image", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Get_User_And_Image([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytUserDataTable, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytRawImageArray, ref int FingerPrintScore, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] ref byte[] bytFingerImageByteArray, ref int UserNo, ref int FingerNo) {
            object[] results = this.Invoke("Get_User_And_Image", new object[] {
                        bytUserDataTable,
                        bytRawImageArray,
                        FingerPrintScore,
                        bytFingerImageByteArray,
                        UserNo,
                        FingerNo});
            FingerPrintScore = ((int)(results[0]));
            bytFingerImageByteArray = ((byte[])(results[1]));
            UserNo = ((int)(results[2]));
            FingerNo = ((int)(results[3]));
        }
        
        /// <remarks/>
        public void Get_User_And_ImageAsync(byte[] bytUserDataTable, byte[] bytRawImageArray, int FingerPrintScore, byte[] bytFingerImageByteArray, int UserNo, int FingerNo) {
            this.Get_User_And_ImageAsync(bytUserDataTable, bytRawImageArray, FingerPrintScore, bytFingerImageByteArray, UserNo, FingerNo, null);
        }
        
        /// <remarks/>
        public void Get_User_And_ImageAsync(byte[] bytUserDataTable, byte[] bytRawImageArray, int FingerPrintScore, byte[] bytFingerImageByteArray, int UserNo, int FingerNo, object userState) {
            if ((this.Get_User_And_ImageOperationCompleted == null)) {
                this.Get_User_And_ImageOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGet_User_And_ImageOperationCompleted);
            }
            this.InvokeAsync("Get_User_And_Image", new object[] {
                        bytUserDataTable,
                        bytRawImageArray,
                        FingerPrintScore,
                        bytFingerImageByteArray,
                        UserNo,
                        FingerNo}, this.Get_User_And_ImageOperationCompleted, userState);
        }
        
        private void OnGet_User_And_ImageOperationCompleted(object arg) {
            if ((this.Get_User_And_ImageCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Get_User_And_ImageCompleted(this, new Get_User_And_ImageCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Find_Employee", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Find_Employee(int DeviceNo, int EmployeeNo, int PrevEmployeeCompanyNo, ref string Names, ref int EmployeeCompanyNo, ref string EmployeeCompanyName) {
            object[] results = this.Invoke("Find_Employee", new object[] {
                        DeviceNo,
                        EmployeeNo,
                        PrevEmployeeCompanyNo,
                        Names,
                        EmployeeCompanyNo,
                        EmployeeCompanyName});
            Names = ((string)(results[0]));
            EmployeeCompanyNo = ((int)(results[1]));
            EmployeeCompanyName = ((string)(results[2]));
        }
        
        /// <remarks/>
        public void Find_EmployeeAsync(int DeviceNo, int EmployeeNo, int PrevEmployeeCompanyNo, string Names, int EmployeeCompanyNo, string EmployeeCompanyName) {
            this.Find_EmployeeAsync(DeviceNo, EmployeeNo, PrevEmployeeCompanyNo, Names, EmployeeCompanyNo, EmployeeCompanyName, null);
        }
        
        /// <remarks/>
        public void Find_EmployeeAsync(int DeviceNo, int EmployeeNo, int PrevEmployeeCompanyNo, string Names, int EmployeeCompanyNo, string EmployeeCompanyName, object userState) {
            if ((this.Find_EmployeeOperationCompleted == null)) {
                this.Find_EmployeeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFind_EmployeeOperationCompleted);
            }
            this.InvokeAsync("Find_Employee", new object[] {
                        DeviceNo,
                        EmployeeNo,
                        PrevEmployeeCompanyNo,
                        Names,
                        EmployeeCompanyNo,
                        EmployeeCompanyName}, this.Find_EmployeeOperationCompleted, userState);
        }
        
        private void OnFind_EmployeeOperationCompleted(object arg) {
            if ((this.Find_EmployeeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Find_EmployeeCompleted(this, new Find_EmployeeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Find_Employee_Names", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Find_Employee_Names([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] bytRawImageArray, int DeviceNo, int EmployeeNo, int EmployeeSmartKeyNo, int EmployeeRFIDNo, ref string Names) {
            object[] results = this.Invoke("Find_Employee_Names", new object[] {
                        bytRawImageArray,
                        DeviceNo,
                        EmployeeNo,
                        EmployeeSmartKeyNo,
                        EmployeeRFIDNo,
                        Names});
            Names = ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Find_Employee_NamesAsync(byte[] bytRawImageArray, int DeviceNo, int EmployeeNo, int EmployeeSmartKeyNo, int EmployeeRFIDNo, string Names) {
            this.Find_Employee_NamesAsync(bytRawImageArray, DeviceNo, EmployeeNo, EmployeeSmartKeyNo, EmployeeRFIDNo, Names, null);
        }
        
        /// <remarks/>
        public void Find_Employee_NamesAsync(byte[] bytRawImageArray, int DeviceNo, int EmployeeNo, int EmployeeSmartKeyNo, int EmployeeRFIDNo, string Names, object userState) {
            if ((this.Find_Employee_NamesOperationCompleted == null)) {
                this.Find_Employee_NamesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFind_Employee_NamesOperationCompleted);
            }
            this.InvokeAsync("Find_Employee_Names", new object[] {
                        bytRawImageArray,
                        DeviceNo,
                        EmployeeNo,
                        EmployeeSmartKeyNo,
                        EmployeeRFIDNo,
                        Names}, this.Find_Employee_NamesOperationCompleted, userState);
        }
        
        private void OnFind_Employee_NamesOperationCompleted(object arg) {
            if ((this.Find_Employee_NamesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Find_Employee_NamesCompleted(this, new Find_Employee_NamesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Ping_Database", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool Ping_Database() {
            object[] results = this.Invoke("Ping_Database", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void Ping_DatabaseAsync() {
            this.Ping_DatabaseAsync(null);
        }
        
        /// <remarks/>
        public void Ping_DatabaseAsync(object userState) {
            if ((this.Ping_DatabaseOperationCompleted == null)) {
                this.Ping_DatabaseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPing_DatabaseOperationCompleted);
            }
            this.InvokeAsync("Ping_Database", new object[0], this.Ping_DatabaseOperationCompleted, userState);
        }
        
        private void OnPing_DatabaseOperationCompleted(object arg) {
            if ((this.Ping_DatabaseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Ping_DatabaseCompleted(this, new Ping_DatabaseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Get_Template_And_ImageCompletedEventHandler(object sender, Get_Template_And_ImageCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Get_Template_And_ImageCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Get_Template_And_ImageCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] bytPreviousTemplate {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
        
        /// <remarks/>
        public int FingerPrintScore {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public int VerifyScoreFingerprintsCompare {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[2]));
            }
        }
        
        /// <remarks/>
        public byte[] bytFingerImageByteArray {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[3]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Show_New_ImageCompletedEventHandler(object sender, Show_New_ImageCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Show_New_ImageCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Show_New_ImageCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] bytFingerImageByteArray {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Get_User_And_ImageCompletedEventHandler(object sender, Get_User_And_ImageCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Get_User_And_ImageCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Get_User_And_ImageCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int FingerPrintScore {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public byte[] bytFingerImageByteArray {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[1]));
            }
        }
        
        /// <remarks/>
        public int UserNo {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[2]));
            }
        }
        
        /// <remarks/>
        public int FingerNo {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[3]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Find_EmployeeCompletedEventHandler(object sender, Find_EmployeeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Find_EmployeeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Find_EmployeeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Names {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public int EmployeeCompanyNo {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public string EmployeeCompanyName {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[2]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Find_Employee_NamesCompletedEventHandler(object sender, Find_Employee_NamesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Find_Employee_NamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Find_Employee_NamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Names {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void Ping_DatabaseCompletedEventHandler(object sender, Ping_DatabaseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Ping_DatabaseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Ping_DatabaseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591