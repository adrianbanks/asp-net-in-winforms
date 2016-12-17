using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace MyWebApplication
{
    public delegate void WebBrowserNavigateErrorEventHandler(object sender, WebBrowserNavigateErrorEventArgs e);

    public class WebBrowser2 : WebBrowser
    {
        private AxHost.ConnectionPointCookie cookie;
        private WebBrowser2EventHelper helper;

        public event WebBrowserNavigateErrorEventHandler NavigateError;

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            // Create an instance of the client that will handle the event
            // and associate it with the underlying ActiveX control.
            helper = new WebBrowser2EventHelper(this);
            cookie = new AxHost.ConnectionPointCookie(ActiveXInstance, helper, typeof(DWebBrowserEvents2));
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            // Disconnect the client that handles the event
            // from the underlying ActiveX control.
            if (cookie != null)
            {
                cookie.Disconnect();
                cookie = null;
            }

            base.DetachSink();
        }

        protected virtual void OnNavigateError(WebBrowserNavigateErrorEventArgs e)
        {
            NavigateError?.Invoke(this, e);
        }

        private class WebBrowser2EventHelper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private readonly WebBrowser2 parent;

            public WebBrowser2EventHelper(WebBrowser2 parent)
            {
                this.parent = parent;
            }

            public void NavigateError(object pDisp, ref object url,
                ref object frame, ref object statusCode, ref bool cancel)
            {
                var eventArgs = new WebBrowserNavigateErrorEventArgs((string) url, (string) frame, (int) statusCode, cancel);
                parent.OnNavigateError(eventArgs);
            }
        }
    }

    [ComImport, Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
     InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
     TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DWebBrowserEvents2
    {
        [DispId(271)]
        void NavigateError(
            [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
            [In] ref object url, [In] ref object frame,
            [In] ref object statusCode, [In, Out] ref bool cancel);
    }

    public class WebBrowserNavigateErrorEventArgs : EventArgs
    {
        public WebBrowserNavigateErrorEventArgs(string url, string frame, int statusCode, bool cancel)
        {
            Url = url;
            Frame = frame;
            StatusCode = statusCode;
            Cancel = cancel;
        }

        public string Url { get; }
        public string Frame { get; }
        public int StatusCode { get; }
        public bool Cancel { get; }
    }
}
