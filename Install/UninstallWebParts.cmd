rem -- undoes deployment of the SharePointLearningKit solution to WSS; use DeleteSolution.cmd to
rem -- fully delete the solution
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"
%SPDIR%\bin\stsadm -o retractsolution -name MLG2007.WebParts.MetaSearch.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLG2007.WebParts.MyChildren.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLG2007.WebParts.MySites.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLG2007.WebParts.Myplanner.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLG2007.WebParts.LRUpload.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLGCalendarList.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MLGUserPreferencesList.wsp -immediate
%SPDIR%\bin\stsadm -o execadmsvcjobs

%SPDIR%\bin\stsadm -o deletesolution -name MLG2007.WebParts.MetaSearch.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLG2007.WebParts.MyChildren.wsp 
%SPDIR%\bin\stsadm -o deletesolution -name MLG2007.WebParts.MySites.wsp 
%SPDIR%\bin\stsadm -o deletesolution -name MLG2007.WebParts.Myplanner.wsp 
%SPDIR%\bin\stsadm -o deletesolution -name MLG2007.WebParts.LRUpload.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGCalendarList.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGUserPreferencesList.wsp
