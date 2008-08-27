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
using Microsoft.SharePoint.Security;

namespace MLG
{
    [CLSCompliant(false)]
    [Guid("F1F754C9-3729-45e7-B099-E77334AFFBC3")]
    partial class MLG : SPFeatureReceiver
    {
        /// <summary>
        ///  These methods are required for deploying Site Definitions 
        ///  and should not be modified. Use SiteProvisioning.cs
        ///  for your custom provisioning code.
        /// </summary>
        private const string visitor = " Visitors";
        private const string owner = " Owners";
        private const string member = " Members";

        private const string readPer = "Read";
        private const string contributePer = "Contribute";
        private const string adminPer = "Full Control";

        #region SharePoint Site Definition generated code

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            SPWeb web = properties.Feature.Parent as SPWeb;
            web.BreakRoleInheritance(false);
           
            //remove the groups from the collection
            string[] groupsNames = new string[web.Groups.Count];
            for (int i = 0; i < web.Groups.Count; i++)
            {
                groupsNames[i] = web.Groups[i].Name;
            }
            for (int j = 0; j < groupsNames.Length; j++)
            {
                web.Groups.Remove(groupsNames[j]); ;
            }
            web.Update();

            //add the site groups
            web.SiteGroups.Add(web.ParentWeb.Title+" "+web.Title + visitor, web.Site.Owner, null, "Use this group to give people read permissions to the SharePoint site: " + web.Title);
            web.SiteGroups.Add(web.ParentWeb.Title+" "+web.Title + member, web.Site.Owner, null, "Use this group to give people contribute permissions to the SharePoint site: " + web.Title);
            web.SiteGroups.Add(web.ParentWeb.Title+" "+web.Title + owner, web.Site.Owner, web.Site.Owner, "Use this group to give people full control permissions to the SharePoint site: "+web.Title);
            
            //add the site groups to the cross site groups
            web.Roles[readPer].AddGroup(web.SiteGroups[web.ParentWeb.Title+" "+web.Title + visitor]);
            web.Roles[contributePer].AddGroup(web.SiteGroups[web.ParentWeb.Title+" "+web.Title + member]);
            web.Roles[adminPer].AddGroup(web.SiteGroups[web.ParentWeb.Title+" "+web.Title + owner]);

            //add the site groups to the site association
            web.AssociatedMemberGroup = web.SiteGroups[web.ParentWeb.Title+" "+web.Title + member];
            web.AssociatedOwnerGroup = web.SiteGroups[web.ParentWeb.Title+" "+web.Title + owner];
            web.AssociatedVisitorGroup = web.SiteGroups[web.ParentWeb.Title+" "+web.Title + visitor];

            //update the web object
            web.Update();

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
