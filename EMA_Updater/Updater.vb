'***************************************************
' Copyright (c) 2010 - 2011
' Original Devs:
' Mike Wohlrab (ECLIPS3)
' James Lichtenstiger (jamezelle)
'
' This program is now open source
' but we have a few requirements:
' 1) This must ALWAYS remaine free
' 2) You must keep all license and credits
' to the original devs in here. i.e. Credits Page
'
' You may contact us by email:
' Mike: Mike@mikewohlrab.com
' Or on IRC:
' irc.andirc.net #DroidEris #Zion
' 
' All Rights Reserved
'***************************************************

Option Strict On
Imports System.IO
Imports System.Net
Module Updater

    Dim strEMA_Location As String = FileSystem.CurDir
    Dim strEMA_APP_EXE As String = FileSystem.CurDir
    Dim strVersionNewTXT, strVersionOldTXT As String
    Dim strEMA_DL_Link As String
    Dim blnOSIsXP As Boolean = False

    '***********************************************************************************************************************************
    'Get os version to set os specific variables.
    '***********************************************************************************************************************************
#Region "Gather OS Information"

    Function GetOSVersion() As String
        Select Case Environment.OSVersion.Platform
            Case PlatformID.Win32S
                Return "Win 3.1"
            Case PlatformID.Win32Windows
                Select Case Environment.OSVersion.Platform
                    Case 0
                        Return "Win95"
                    Case CType(10, PlatformID)
                        Return "win98"
                    Case CType(90, PlatformID)
                        Return "WinME"
                    Case Else
                        Return "Unknown"
                End Select
            Case PlatformID.Win32NT
                Select Case Environment.OSVersion.Version.Major
                    Case 3
                        Return "NT 3.51"
                    Case 4
                        Return "NT 4.0"
                    Case 5
                        Select Case _
                            Environment.OSVersion.Version.Minor
                            Case 0
                                Return "Win2000"
                                'this is the one we are looking for in particular
                            Case 1
                                blnOSIsXP = True
                                Return "WinXP"
                                'dont need to go on from here
                            Case Else
                                Return "Unknown"
                        End Select
                    Case Else
                        Return "Unknown"
                End Select
            Case Else
                Return "Unknown"
        End Select
    End Function

#End Region

    Sub Main()

        '****DOWNLOAD NEW VERSION FILE****
        'Download for New Version File
        Dim strVersionDLFile As String = "http://android.grdlock.net/index.php?action=downloadfile&filename=newversion.txt&directory=ECLIPS3s%20Bag%20of%20Tricks/TESTING_DO_NOT_DOWNLOAD&"

        Dim wr As HttpWebRequest = CType(WebRequest.Create(strVersionDLFile), HttpWebRequest)
        Dim ws As HttpWebResponse = CType(wr.GetResponse(), HttpWebResponse)
        Dim str As Stream = ws.GetResponseStream()
        Dim intFileSize As Integer
        intFileSize = CInt(ws.ContentLength)
        Dim inBuf(intFileSize) As Byte
        Dim bytesToRead As Integer = intFileSize
        Dim bytesRead As Integer = 0

        While bytesToRead > 0
            Dim n As Integer = str.Read(inBuf, bytesRead, bytesToRead)
            If n = 0 Then
                Exit While
            End If

            'delay which is needed for xp os support. if the file downloads too fast, then it will say
            'that the file cannot be downloaded / found and to try a different file
            'EDIT By JAMES -  0.5 seems to be the sweet spot
            GetOSVersion()
            If blnOSIsXP = True Then
                System.Threading.Thread.Sleep(800)
            End If
            bytesRead += n
            bytesToRead -= n
            'END EDIT By JAMES

        End While

        'Writes new file
        Dim fstr As New FileStream(strEMA_Location & "/newversion.txt", FileMode.OpenOrCreate, FileAccess.Write)
        fstr.Write(inBuf, 0, bytesRead)
        str.Close()
        fstr.Close()


        '****NEW VERSION FILE****
        'read what version new file is
        Try
            Dim sr As StreamReader

            sr = New StreamReader(strEMA_Location & "/newversion.txt")
            strVersionNewTXT = sr.ReadLine()
            sr.Close()

        Catch ex As Exception
            MsgBox("Unable to check new file version. Either the file is not there or it was deleted. Please make sure you have an active internet connection and try the updater again.")
        End Try


        '****OLD VERSION FILE****
        'read what version old file is
        Try
            Dim sr As StreamReader
            sr = New StreamReader(strEMA_Location & "/oldversion.txt")

            strVersionOldTXT = sr.ReadLine()
            sr.Close()

        Catch ex As Exception
            'If the app can't find the oldversion.txt file, a default value of 0.0 is used so app will update anyways
            strVersionOldTXT = "0.0"
            ' MsgBox("Unable to check old file version. Most likely the file is not there, so the app will update anyways.")
        End Try


        Try

            '****CHECK VERSION FILES****
            If CDbl(strVersionOldTXT) < CDbl(strVersionNewTXT) Then

                'perform update
                strEMA_DL_Link = "http://android.grdlock.net/index.php?action=downloadfile&filename=Eris_Master_App.exe&directory=ECLIPS3s%20Bag%20of%20Tricks/TESTING_DO_NOT_DOWNLOAD&"

                Dim wr2 As HttpWebRequest = CType(WebRequest.Create(strEMA_DL_Link), HttpWebRequest)
                Dim ws2 As HttpWebResponse = CType(wr2.GetResponse(), HttpWebResponse)
                Dim str2 As Stream = ws2.GetResponseStream()
                Dim intFileSize2 As Integer
                intFileSize2 = CInt(ws2.ContentLength)
                Dim inBuf2(intFileSize2) As Byte
                Dim bytesToRead2 As Integer = intFileSize2
                Dim bytesRead2 As Integer = 0

                While bytesToRead2 > 0
                    Dim n As Integer = str2.Read(inBuf2, bytesRead2, bytesToRead2)
                    If n = 0 Then
                        Exit While
                    End If

                    'delay which is needed for xp os support. if the file downloads too fast, then it will say
                    'that the file cannot be downloaded / found and to try a different file
                    'EDIT By JAMES -  0.5 seems to be the sweet spot
                    GetOSVersion()
                    If blnOSIsXP = True Then
                        System.Threading.Thread.Sleep(800)
                    End If

                    bytesRead2 += n
                    bytesToRead2 -= n
                    'END EDIT By JAMES

                End While

                'Writes new file
                Dim fstr2 As New FileStream(strEMA_Location & "/Eris_Master_App_Update.exe", FileMode.OpenOrCreate, FileAccess.Write)
                'EDIT BY JAMES                                                  ^^^^^ changed name
                fstr2.Write(inBuf2, 0, bytesRead2)
                str2.Close()
                fstr2.Close()
                'added code by Jamezelle

                Dim strUpdateAppPath As String = strEMA_Location & "/Eris_Master_App_Update.exe"
                Try
                    If File.Exists(strUpdateAppPath) = True Then
                        Dim strOriginalAppPathAndName As String = strEMA_Location & "/Eris_Master_App.exe"
                        Dim strOldAppPathName As String = strEMA_Location & "/Eris_Master_App_Old.exe"
                        Try
                            Rename(strOriginalAppPathAndName, strOldAppPathName)
                            Rename(strUpdateAppPath, strOriginalAppPathAndName)
                        Catch ex As Exception
                            ' MsgBox("cannot find the original app file.")
                            Rename(strUpdateAppPath, strOriginalAppPathAndName)
                        End Try

                    ElseIf File.Exists(strUpdateAppPath) = False Then
                        MsgBox("Update failed to find Eris_Master_App_Update.exe! Please make sure you have an active internet connection and try to update the proram again.")

                    End If
                Catch ex As Exception
                    MsgBox("Unable to rename the files needed. Please make sure you have an active internet connection and try the updater again.")
                End Try
                'End of added code by jamezelle -feel free to test/change
            End If

            If File.Exists(strEMA_Location & "/oldversion.txt") Then
                FileSystem.Kill(strEMA_Location & "/oldversion.txt")
            End If

            FileSystem.Rename(strEMA_Location & "/newversion.txt", strEMA_Location & "/oldversion.txt")

            MsgBox("Update complete. You are now up to date. You may now use the EMA application.")

        Catch ex As Exception

        End Try

    End Sub

End Module
