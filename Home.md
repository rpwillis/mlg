This project has now moved to GitHub at https://github.com/rpwillis/mlg

## Announcements

* SLK 1.3.1 support added to the 1.1 release. [release:23591](release_23591)
* I've started a 1.1 release. [release:23591](release_23591)
* Richard Willis has accepted the role of coordinator on the Learning Gateway project
* The license for Learning Gateway has been changed to the Microsoft Public License (MsPL) which is an OSI certified Open Source license.
* The source code for Learning Gateway has been uploaded into the Codeplex source control system in anticipation of accepting patches and improvements from the community.
* Community contributions are welcome!

## Overview

The Microsoft Learning Gateway Refresh for MOSS 2007 and WSS 3.0 is available for download. For in additional information, non-technical discussions, and related resources, please visit http://www.learninggateway.net.  For technical discussions and bug reports, please use the Discussions and Issue Tracker tabs on this site.

Functionality

+•IMS Upload+
The IMS Upload web part is an import front-end to a SharePoint document library that populates the document library with learning-specific columns defined within the IMS or SCORM metadata.

+•Meta Search+
The Meta Search web part provides metadata search on IMS or SCORM learning objects in a document library.

+•My Classes+
My Classes populates a list of the class sites that the user is a member of.  It is based on My Teams web part from MLG 2005.  

+•My Children+
My Children retrieves the logged on parent's children and their pictures and provides a filter to other web parts on the page that reflect a parent view of data for their child.

+•My Planner+
My Planner shows the current users' Exchange mailbox & calendars, SLK events for the current user, and SharePoint events for the current user.

## New features in the June 2007 Refresh

* Deployment process, templates and application web parts rebuilt for compatibility with MOSS, SharePoint Portal Server 2007 and WSS 3.0.
* Deployment process refined for ISA 2006, SQL 2005 and Exchange 2007.  
* Provision to introduce Forms Server as an optional component.
* Introduction of ISA 2006 forms based authentication.

* Introduces SharePoint Learning Kit (SLK) at appropriate points.
* Class Server has been retired.
* A new document library based Learning Object Repository has been added with automatic META data extraction and complimentary META based search.

* Introduction of Blogs, Wiki’s and My Sites.
* Better use and strategic placement of Discussions, Surveys and Lists.
* Better use of current and global navigation.
* Improved granularity of audience targeting for documents, list items and news.

* Introduction of a district or LEA, top-level portal in a multi-school context.
* Unlimited number of member roles, replacing static parent, teacher, student roles.
* Improved cross-role / cross-institution collaboration.

* Support for more powerful, template based customization introduced via ASPX Masterpages.
* New deployment guide.
* Comprehensive partner training workshop resources.

**Expectations**

* There are no upgrade scenarios considered from previous versions of Learning Gateway or SharePoint.  The “framework” nature of the Microsoft Learning Gateway makes such upgrades and migrations an individual project, not a system-wide tool.  Customers will be looking to you for leadership and guidance in this process.
* This release of Learning Gateway runs on MOSS 2007, WSS 3.0, Exchange 2007 and ISA 2006.  
* This release is intended to be an example and demonstration architecture only.   Most customers should not try this without assistance! Deployment by a suitably qualified systems integrator is required as this set of framework and accelerator code can not anticipate all customer requirements or environments and some customization will almost certainly be necessary.  
* Deployment tools are not included.