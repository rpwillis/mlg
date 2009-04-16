rem -- adds and deploys the MLG solution to WSS
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MasterpageSetter.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGTeacher.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGStudent.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGParent.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGSiteContainer.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGClass.wsp

%SPDIR%\bin\stsadm -o addsolution -filename Solutions\MLGSchool.wsp

@ECHO OFF
ECHO Now deploy solutions through SharePoint Central Administration.
