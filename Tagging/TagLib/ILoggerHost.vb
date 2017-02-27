Public Interface ILoggerHost
    Sub LS(ByVal txt As String)
    Sub LX(ByVal txt As String)
    Sub Achtung(ByVal txt As String)
    Property LogLevel As Integer
End Interface
