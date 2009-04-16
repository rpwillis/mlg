rem -- Enable webparts features' on the selected URL.
@SET SPDIR="%commonprogramfiles%\common files\microsoft shared\web server extensions\12"

@ECHO OFF
if "%1" EQU "" GOTO error
@ECHO ON

%SPDIR%\bin\stsadm -o activatefeature -name mysites -url "%1"
%SPDIR%\bin\stsadm -o activatefeature -name mychildren -url "%1"
%SPDIR%\bin\stsadm -o activatefeature -name metasearch -url "%1"
%SPDIR%\bin\stsadm -o activatefeature -name myplanner -url  "%1"
%SPDIR%\bin\stsadm -o activatefeature -name LRUpload -url  "%1"
%SPDIR%\bin\stsadm -o activatefeature -name CalendarList -url  "%1"
%SPDIR%\bin\stsadm -o activatefeature -name UserPreferencesList -url  "%1"
@ECHO OFF
GOTO done

:error
@ECHO OFF
ECHO Please enter the site collection URL that you want to enable the webparts on.
ECHO The command should look like 
ECHO               EnableWebpartFeatures.exe http://localhost

:done
