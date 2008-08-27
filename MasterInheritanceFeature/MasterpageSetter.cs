using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Security;
using System.Security.Permissions;
using System.Globalization;
using Microsoft.Win32;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Security;

namespace MasterPageSetter
{
    [CLSCompliant(false)]
    [Guid("F1F754C9-3729-45e7-B099-E77334AFFBD5")]
    partial class MasterInheritanceFeature : SPFeatureReceiver
    {
        private const string masterPageName = "/MLGSchool.master";

        #region SharePoint Site Definition generated code

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            SPWeb web = properties.Feature.Parent as SPWeb;

            //get the site collection URL
            string siteCollectionURL = web.Site.Url;

            //temp variable that will holds the School Site URL
            string schoolWebURL = null;

            //if the web of type MLGSchool, set its MasterURL and Custom MasterURl
            if (string.Compare(web.WebTemplate, "MLGSchool", true) == 0)
            {
                schoolWebURL = web.Url;
                try
                {
                    string masterURL = schoolWebURL.Substring(siteCollectionURL.Length) + masterPageName;
                    web.MasterUrl = masterURL;
                    web.CustomMasterUrl = masterURL;
                    web.Update();
                }
                catch { }

            }
            else
            {
                //in case this is not a school site, check if it is a publishing web,
                //and if so set the MasterURL, and customMusterURl to inherit from the parent site.
                try
                {
                    if (PublishingWeb.IsPublishingWeb(web))
                    {
                        PublishingWeb pWeb = PublishingWeb.GetPublishingWeb(web);
                        pWeb.MasterUrl.SetInherit(true, true);
                        pWeb.CustomMasterUrl.SetInherit(true, true);
                        pWeb.Update();
                    }
                }
                catch { }
            }
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }
        #endregion
    }
}
