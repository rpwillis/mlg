rem -- adds and deploys the MLG solution to WSS
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLG2007.WebParts.MetaSearch.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLG2007.WebParts.MyChildren.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLG2007.WebParts.MySites.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLG2007.WebParts.Myplanner.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLG2007.WebParts.LRUpload.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGCalendarList.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGUserPreferencesList.wsp

@ECHO OFF
ECHO Now deploy solutions through SharePoint Central Administration.
