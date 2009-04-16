@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"
%SPDIR%\bin\stsadm -o upgradesolution -name MLG2007.WebParts.MetaSearch.wsp -filename Solutions\MLG2007.WebParts.MetaSearch.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name MLG2007.WebParts.MyChildren.wsp -filename Solutions\MLG2007.WebParts.MyChildren.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name MLG2007.WebParts.MySites.wsp -filename Solutions\MLG2007.WebParts.MySites.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name MLG2007.WebParts.Myplanner.wsp -filename Solutions\MLG2007.WebParts.Myplanner.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name MLG2007.WebParts.LRUpload.wsp -filename Solutions\MLG2007.WebParts.LRUpload.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name mlgcalendarlist.wsp -filename Solutions\MLGCalendarList.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGUserPreferencesList.wsp -filename Solutions\MLGUserPreferencesList.wsp -immediate
%SPDIR%\bin\stsadm -o execadmsvcjobs

@ECHO OFF
