rem -- undoes deployment of the SharePointLearningKit solution to WSS; use DeleteSolution.cmd to
rem -- fully delete the solution
net start spadmin
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"
%SPDIR%\bin\stsadm -o retractsolution -name MLGSchool.wsp -immediate -allcontenturls
%SPDIR%\bin\stsadm -o retractsolution -name MLGTeacher.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MLGParent.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MLGStudent.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MLGSiteContainer.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MLGClass.wsp -immediate
%SPDIR%\bin\stsadm -o retractsolution -name MasterpageSetter.wsp -immediate -allcontenturls

%SPDIR%\bin\stsadm -o execadmsvcjobs

%SPDIR%\bin\stsadm -o deletesolution -name MLGSchool.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGTeacher.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGParent.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGStudent.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGSiteContainer.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MLGClass.wsp
%SPDIR%\bin\stsadm -o deletesolution -name MasterpageSetter.wsp
