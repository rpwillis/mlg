@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"

%SPDIR%\bin\stsadm -o upgradesolution -name MasterpageSetter.wsp -filename Solutions\MasterpageSetter.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o upgradesolution -name MLGTeacher.wsp -filename Solutions\MLGTeacher.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGStudent.wsp -filename Solutions\MLGStudent.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGParent.wsp -filename Solutions\MLGParent.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGSiteContainer.wsp -filename Solutions\MLGSiteContainer.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGClass.wsp -filename Solutions\MLGClass.wsp -immediate
%SPDIR%\bin\stsadm -o upgradesolution -name MLGSchool.wsp -filename Solutions\MLGSchool.wsp -immediate -allowgacdeployment
%SPDIR%\bin\stsadm -o execadmsvcjobs
@ECHO OFF


