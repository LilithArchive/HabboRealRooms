Imports System.IO
Imports System.Security.Cryptography

Class MainWindow
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        IO.Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
        UpdateBlockStatus()
    End Sub

    Private Sub BlockButton_Click(sender As Object, e As RoutedEventArgs) Handles BlockButton.Click
        If BlockButton.Content.ToString.StartsWith("Bloquear") Then
            Try
                If IO.File.Exists(GetClientPath() & "\HabboAir_ORIGINAL.swf") = False Then
                    BackupOriginalClient()
                End If
                CopyModdedClient()
            Catch
                MsgBox("Error al bloquear el contenido.", MsgBoxStyle.Critical, "Error")
            End Try
        Else
            Try
                RestoreOriginalClient()
            Catch
                MsgBox("Error al desbloquear el contenido.", MsgBoxStyle.Critical, "Error")
            End Try
        End If
        UpdateBlockStatus()
    End Sub

    Function CopyModdedClient()
        IO.File.WriteAllBytes(GetClientPath() & "\HabboAir.swf", My.Resources.HabboAir)
    End Function

    Function BackupOriginalClient()
        IO.File.WriteAllBytes(GetClientPath() & "\HabboAir_ORIGINAL.swf", IO.File.ReadAllBytes(GetClientPath() & "\HabboAir.swf"))
    End Function

    Function RestoreOriginalClient()
        IO.File.Delete(GetClientPath() & "\HabboAir.swf")
        IO.File.Move(GetClientPath() & "\HabboAir_ORIGINAL.swf", GetClientPath() & "\HabboAir.swf")
    End Function

    Private Sub AboutButton_Click(sender As Object, e As RoutedEventArgs) Handles AboutButton.Click
        MsgBox("Esta herramienta fue creada para bloquear la visualizacion de salas que usan bots en los resultados del navegador, dandole mas visibilidad a salas reales.", MsgBoxStyle.Information, "Acerca")
    End Sub

    Sub UpdateBlockStatus()
        Try
            If GetLocalHabboAirMD5() = GetModdedHabboAirMD5() Then
                BlockButton.Background = Brushes.DarkRed
                BlockButton.Content = "Desbloquear salas que usan bots"
            Else
                BlockButton.Background = Brushes.Green
                BlockButton.Content = "Bloquear salas que usan bots"
            End If
        Catch
            BlockButton.Background = Brushes.Green
            BlockButton.Content = "Bloquear salas que usan bots"
        End Try
    End Sub

    Function GetClientPath() As String
        Try
            Dim LocalAppDataPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Sulake\Habbo Launcher\HabboFlash"
            Dim AppDataPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Habbo Launcher\downloads\air"
            AppDataPath += "\" & IO.Directory.GetDirectories(AppDataPath).Max(Function(d) New DirectoryInfo(d).Name)
            If Directory.Exists("META-INF\AIR") Then
                Return Directory.GetCurrentDirectory
            End If
            If Directory.Exists(AppDataPath & "\META-INF\AIR") Then
                Return AppDataPath
            End If
            Throw New Exception("Client not found")
        Catch
            MsgBox("Habbo Client no encontrado." & vbNewLine & "Puedes descargarlo desde la web de Habbo.", MsgBoxStyle.Critical, "Error")
            Environment.Exit(0)
        End Try
    End Function

    Function GetLocalHabboAirMD5() As String
        Return GetFileMD5(GetClientPath() & "\HabboAir.swf")
    End Function

    Function GetModdedHabboAirMD5() As String
        Return GetBytesMD5(My.Resources.HabboAir)
    End Function

    Private Function GetFileMD5(ByVal filename As String) As String
        Using md5 As MD5 = MD5.Create()
            Using stream = IO.File.OpenRead(filename)
                Return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", String.Empty)
            End Using
        End Using
    End Function

    Private Function GetBytesMD5(ByVal data As Byte()) As String
        Using md5 As MD5 = MD5.Create()
            Return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", String.Empty)
        End Using
    End Function

End Class
