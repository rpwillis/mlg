Option Explicit
Dim oFS, oDDF, args, oFestures, iFeatureCount

dim arFeatures(20)
iFeatureCount = 0

set args = WScript.Arguments

' Create the File System Object
Set oFS = CreateObject("Scripting.FileSystemObject")

Set oDDF = oFS.CreateTextFile(args(0) + "cab.ddf", true)
oDDF.WriteLine ";"
oDDF.WriteLine ".Set CabinetNameTemplate=" + "MLG2007.WebParts."+args(1) + ".wsp"
oDDF.WriteLine ".set DiskDirectoryTemplate=CDROM ; All cabinets go in a single directory"
oDDF.WriteLine ".Set CompressionType=MSZIP;** All files are compressed in cabinet files"
oDDF.WriteLine ".Set UniqueFiles='ON'"
oDDF.WriteLine ".Set Cabinet=on"
oDDF.WriteLine ".Set DiskDirectory1=."


' Add the assemblies
EnumFolder args(0) + "TEMPLATE", "TEMPLATE"
oDDF.WriteLine """"+args(0) + "manifest.xml" +""""+ vbTab + "manifest.xml"

'add the dlls

oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.WebParts.MyPlanner.dll" +""""+ vbTab + "MLG2007.WebParts.MyPlanner.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.Calendar.dll" +""""+ vbTab + "MLG2007.Helper.Calendar.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.ListSearch.dll" +""""+ vbTab + "MLG2007.Helper.ListSearch.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.CalendarStore.dll" +""""+ vbTab + "MLG2007.Helper.CalendarStore.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.SharePoint.dll" +""""+ vbTab + "MLG2007.Helper.SharePoint.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.SharePointLearningKit.dll" +""""+ vbTab + "MLG2007.Helper.SharePointLearningKit.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.Exchange.dll" +""""+ vbTab + "MLG2007.Helper.Exchange.dll"
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\"+ "MLG2007.Helper.UserPreferences.dll" +""""+ vbTab + "MLG2007.Helper.UserPreferences.dll"

oDDF.WriteLine ";*** <the end>"

oDDF.Close

sub EnumFolder(sFolder, sRelativePath)
    
    dim oFolder, oFolders, oSub, oFile
    set oFolder = oFS.GetFolder(sFolder)
    
    if (sRelativePath = "TEMPLATE") then sRelativePath = ""
    if (sRelativePath = "FEATURES") then sRelativePath = ""
    if (sRelativePath <> "") then sRelativePath = sRelativePath + "\"

    for each oFile in oFolder.Files
    dim index
    index=InStr(oFile.Name,".scc")
        if index=0 then
            oDDF.WriteLine """" + oFile.Path + """" + vbTab + """" + sRelativePath + oFile.Name + """"
        end if
    next

    for each oSub in oFolder.SubFolders
        EnumFolder oSub.Path, sRelativePath + oSub.Name
    next
    
end sub