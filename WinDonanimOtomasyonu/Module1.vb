Module Module1
    Public strConnStr As String = "Provider=Microsoft.Ace.OleDb.12.0;Data Source=" & _
    Application.StartupPath & "\Database.mdb;"

    'frmOrderer i�in..
    Public intOrdererID As Integer
    Public strOrderer As String = Nothing
    Public boolSelectOrderer As Boolean = False

    'frmBorclar i�in..
    Public intBorcluID As Integer
    Public strBorclu As String = Nothing
End Module
