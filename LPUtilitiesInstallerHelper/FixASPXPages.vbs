Const ForReading = 1
Const ForWriting = 2
Set oFS = CreateObject("Scripting.FileSystemObject")

EnumFolder(Wscript.Arguments(0))

sub EnumFolder(sFolder)
    
    dim oFolder, oFolders, oSub, oFile
    set oFolder = oFS.GetFolder(sFolder)
    
    for each oFile in oFolder.Files
    dim index
    index=InStr(oFile.Name,".aspx")
        if index>0 then
            EditFile(sFolder+"\"+oFile.Name)
        end if
    next

    
end sub


sub EditFile(filename)

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile(filename, ForReading)
strText = objFile.ReadAll
objFile.Close
'replace the dll with a blank text
strNewText = Replace(strText,", MLG2007.LGUtilities", "")
'add the assembly line at the top of the file
strNewText="<%@ Assembly Name=""MLG2007.LGUtilities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d49d5b75baaca5c2"" %>"+ vbCrLf +strNewText

Set objFile = objFSO.OpenTextFile(filename, ForWriting)
objFile.WriteLine strNewText
objFile.Close

end sub

