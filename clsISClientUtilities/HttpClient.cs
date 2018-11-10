using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace InteractPayrollClient
{
    public class HttpClient : ClientBase<IRequestChannel>
    {
        AsyncCallback ascAsyncCallback;
        static System.ServiceModel.Channels.Message respond;
        static bool pvtblMessageCompleted = false;

        //60 Seconds
        static TimeSpan myTimeSpan = new TimeSpan(600000000);
        //60 Seconds
        static TimeSpan myTimeSpanConnection = new TimeSpan(600000000);
        static System.ServiceModel.Channels.Binding myBinding;

        public static bool blnConnectionFailure = false;
        public static bool blnTimeoutFailure = false;
        public static bool blnOtherFailure = false;

        public HttpClient(string baseUri, bool keepAliveEnabled)
            : this(new Uri(baseUri), keepAliveEnabled)
        {
            ascAsyncCallback = new AsyncCallback(AsyncCallBackFunction);

            respond = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, String.Empty);
        }

        public HttpClient(Uri baseUri, bool keepAliveEnabled)
            : base(HttpClient.CreatePoxBinding(keepAliveEnabled), new EndpointAddress(baseUri))
        {
            ascAsyncCallback = new AsyncCallback(AsyncCallBackFunction);

            respond = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, String.Empty);
        }

        public System.ServiceModel.Channels.Message Request(Uri requestUri, string httpMethod)
        {
            System.ServiceModel.Channels.Message request = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, String.Empty);
            request.Headers.To = requestUri;

            HttpRequestMessageProperty property = new HttpRequestMessageProperty();
            property.Method = httpMethod;

            property.SuppressEntityBody = true;
            request.Properties.Add(HttpRequestMessageProperty.Name, property);

            this.Channel.BeginRequest(request, ascAsyncCallback, this);

            pvtblMessageCompleted = false;

            while (pvtblMessageCompleted == false)
            {
                Application.DoEvents();
            }

            return respond;
        }

        private void AsyncCallBackFunction(IAsyncResult ar)
        {
            try
            {
                respond = this.Channel.EndRequest(ar);
            }
            catch(Exception ex)
            {
                if (ex.Message.IndexOf("has exceeded the allotted timeout") > -1)
                {
                    blnTimeoutFailure = true;
                }
                else
                {
                    if (ex.Message.IndexOf("was no endpoint listening") > -1)
                    {
                        blnConnectionFailure = true;
                    }
                    else
                    {
                        blnOtherFailure = true;
                    }
                }
            }

            pvtblMessageCompleted = true;
        }

        public System.ServiceModel.Channels.Message Request(Uri requestUri, string httpMethod, object entityBody)
        {
            System.ServiceModel.Channels.Message request = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, String.Empty, entityBody);
            request.Headers.To = requestUri;

            HttpRequestMessageProperty property = new HttpRequestMessageProperty();
            property.Method = httpMethod;

            request.Properties.Add(HttpRequestMessageProperty.Name, property);
            return this.Channel.Request(request);
        }

        public System.ServiceModel.Channels.Message Get(Uri requestUri)
        {
            blnConnectionFailure = false;
            blnTimeoutFailure = false;
            blnOtherFailure = false;

            return Request(requestUri, "GET");
        }

        public System.ServiceModel.Channels.Message Post(Uri requestUri, object body)
        {
            blnConnectionFailure = false;
            blnTimeoutFailure = false;
            blnOtherFailure = false;

            return Request(requestUri, "POST", body);
        }

        public static System.ServiceModel.Channels.Binding CreatePoxBinding(bool keepAliveEnabled)
        {
            TextMessageEncodingBindingElement encoder = new TextMessageEncodingBindingElement(MessageVersion.None, Encoding.UTF8);

            encoder.ReaderQuotas.MaxArrayLength = 100000000;

            HttpTransportBindingElement transport = new HttpTransportBindingElement();
            transport.ManualAddressing = true;
            transport.KeepAliveEnabled = keepAliveEnabled;

            transport.MaxBufferPoolSize = 66665536;
            transport.MaxBufferSize = 66665536;
            transport.MaxReceivedMessageSize = 66665536;

            myBinding = new CustomBinding(new BindingElement[] { encoder, transport });

            myBinding.SendTimeout = myTimeSpan;
            myBinding.OpenTimeout = myTimeSpanConnection;
            myBinding.ReceiveTimeout = myTimeSpan;

            return myBinding;
        }
    }
}
