rem -- Enable webparts features' on the selected URL.
@SET SPDIR="%commonprogramfiles%\microsoft shared\web server extensions\12"

@ECHO OFF
if "%1" EQU "" GOTO error
@ECHO ON

%SPDIR%\bin\stsadm -o deploysolution -name MLGCalendarList.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLGUserPreferencesList.wsp -immediate -force
%SPDIR%\bin\stsadm -o deploysolution -name MLG2007.WebParts.MetaSearch.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o deploysolution -name MLG2007.WebParts.MyChildren.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o deploysolution -name MLG2007.WebParts.MySites.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o deploysolution -name MLG2007.WebParts.Myplanner.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o deploysolution -name MLG2007.WebParts.LRUpload.wsp -immediate  -allowgacdeployment -url "%1" -force
%SPDIR%\bin\stsadm -o execadmsvcjobs

@ECHO OFF
GOTO done

:error
@ECHO OFF
ECHO Please enter the web application URL that you want to deploy the webparts to.
ECHO The command should look like 
ECHO               DeployTemplates.cmd http://localhost

:done
