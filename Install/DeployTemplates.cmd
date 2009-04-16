rem -- Enable webparts features' on the selected URL.
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"

@ECHO OFF
if "%1" EQU "" GOTO error
@ECHO ON

%SPDIR%\bin\stsadm -o deploysolution -name MasterpageSetter.wsp -immediate -force -allowgacdeployment -url "%1"
%SPDIR%\bin\stsadm -o deploysolution -name MLGTeacher.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGStudent.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGParent.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGSiteContainer.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGClass.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGSchool.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o execadmsvcjobs


@ECHO OFF
GOTO done

:error
@ECHO OFF
ECHO Please enter the web application URL that you want to deploy the templates to.
ECHO The command should look like 
ECHO               DeployTemplates.exe http://localhost

:done
