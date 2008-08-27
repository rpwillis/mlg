Option Explicit
Dim oFS, oDDF, args, oFestures, iFeatureCount

dim arFeatures(20)
iFeatureCount = 0

set args = WScript.Arguments

' Create the File System Object
Set oFS = CreateObject("Scripting.FileSystemObject")

Set oDDF = oFS.CreateTextFile(args(0) + "cab.ddf", true)
oDDF.WriteLine ";"
oDDF.WriteLine ".Set CabinetNameTemplate=" + "MLG2007.LGUtilities.wsp"
oDDF.WriteLine ".set DiskDirectoryTemplate=CDROM ; All cabinets go in a single directory"
oDDF.WriteLine ".Set CompressionType=MSZIP;** All files are compressed in cabinet files"
oDDF.WriteLine ".Set UniqueFiles='ON'"
oDDF.WriteLine ".Set Cabinet=on"
oDDF.WriteLine ".Set DiskDirectory1=."


' Add the assemblies
EnumFolder args(0) +"bin\"+args(2)+"\pre", "layouts\1033\LGUtilities"
oDDF.WriteLine """"+args(0) + "manifest.xml" +""""+ vbTab + "manifest.xml"

'add the dll
oDDF.WriteLine """"+args(0) +"bin\"+args(2)+"\pre\bin\"+ "MLG2007.LGUtilities.dll" +""""+ vbTab + "MLG2007.LGUtilities.dll"
oDDF.WriteLine ";*** <the end>"

oDDF.Close

sub EnumFolder(sFolder, sRelativePath)
    
    dim oFolder, oFolders, oSub, oFile
    set oFolder = oFS.GetFolder(sFolder)
    
    if (sRelativePath <> "") then sRelativePath = sRelativePath + "\"

    for each oFile in oFolder.Files
    dim index
    index=InStr(oFile.Name,".scc")
        if index=0 then
            index=InStr(oFile.Name,".pdb")
            if index=0 then
                 index=InStr(oFile.Name,".compiled")
                 if index=0 then
                     index=InStr(oFile.Name,"App.config")
                     if index=0 then
                         oDDF.WriteLine """" + oFile.Path + """" + vbTab + """" + sRelativePath + oFile.Name + """"
                     end if
                 end if
            end if
        end if
    next

    for each oSub in oFolder.SubFolders
    if(oSub.Name <> "bin") then    EnumFolder oSub.Path, sRelativePath + oSub.Name
    next
    
end sub