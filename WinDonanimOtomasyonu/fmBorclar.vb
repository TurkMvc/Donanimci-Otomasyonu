#Region "imports"
Imports PureComponents.NicePanel
Imports System
Imports System.Data
Imports System.Exception
Imports System.Data.OleDb
#End Region

Public Class fmBorclar

#Region "Yerel Degiskenler"
    Private myConn As OleDbConnection
    Private myCmd As OleDbCommand
    Private myDR As OleDbDataReader

    Private OdenenBorclar As String
    Private BorcEkle As Single
#End Region

#Region "Metotlar"

    'Borclular� listelemek..
    Private Sub GetBorclular()
        Try
            myConn = New OleDbConnection
            myConn.ConnectionString = Module1.strConnStr

            myCmd = New OleDbCommand
            myCmd.Connection = myConn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandText = "SELECT BorcluID,BorcluAdSoy FROM tblBorclar"

            Dim myDR As OleDbDataReader

            Me.lstBorclar.Items.Clear()

            myConn.Open()
            myDR = myCmd.ExecuteReader

            Do While myDR.Read
                Dim objListBoxNesnesi2 As ListBoxNesnesi2
                objListBoxNesnesi2 = New ListBoxNesnesi2(myDR.Item("BorcluAdSoy").ToString, CInt(myDR.Item("BorcluID")))

                Me.lstBorclar.Items.Add(objListBoxNesnesi2)
            Loop

        Catch exOle As OleDbException
            MessageBox.Show(exOle.Message, "OLEDB HATA !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "GENEL HATA !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            myConn.Close()
        End Try
    End Sub

    'Odenen borclar� okumak..
    Private Sub GetBorcDetails()
        Try
            myConn = New OleDbConnection
            myConn.ConnectionString = Module1.strConnStr

            myCmd = New OleDbCommand
            myCmd.Connection = myConn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandText = "SELECT * FROM tblBorclar where BorcluID=@borcluid"

            Dim myDR As OleDbDataReader

            Me.lstOdenenler.Items.Clear()
            Me.lblBorcBitti.Text = Nothing

            Dim objListBoxNesnesi2 As New ListBoxNesnesi2
            objListBoxNesnesi2 = CType(Me.lstBorclar.SelectedItem, ListBoxNesnesi2)


            myCmd.Parameters.AddWithValue("@borcluid", objListBoxNesnesi2.ID)

            myConn.Open()
            myDR = myCmd.ExecuteReader

            OdenenBorclar = Nothing

            Do While myDR.Read
                Me.txtBorcluAdSoy.Text = myDR.Item("BorcluAdSoy")
                Me.txtToplamBorc.Text = myDR.Item("ToplamBorc")
                OdenenBorclar = myDR.Item("OdenenBorc")
            Loop

            If Not OdenenBorclar = Nothing Then
                GetOdenenBorc()
                SetToplam()
            End If

        Catch exOle As OleDbException
            MessageBox.Show(exOle.Message, "OLEDB HATA !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "GENEL HATA !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            myConn.Close()
        End Try
    End Sub

    'Odenen miktarlar� Odenen borclar listesine aktarmak i�in par�alamak..
    Private Sub GetOdenenBorc()
        Dim X As String = Nothing
        Dim K As Integer = Len(OdenenBorclar)

        'Odenmi� borclar�n listelenmesi..
        For i As Integer = Len(OdenenBorclar) To 2 Step -1
            X = Mid(OdenenBorclar, i, 1)
            If X = "/" Then
                If K = Len(OdenenBorclar) Then
                    X = Mid(OdenenBorclar, i + 1, K - i)
                Else
                    X = Mid(OdenenBorclar, i + 1, K - i - 1)
                End If
                K = i
                Me.lstOdenenler.Items.Add(X)
            End If
        Next

    End Sub

    Private Sub SetToplam()
        Dim Odenenler As Single = 0

        'Kalan borc hesaplama..
        For i As Integer = 0 To Me.lstOdenenler.Items.Count - 1
            Odenenler += Me.lstOdenenler.Items(i)
        Next

        Dim KalanBorc As Single = 0
        KalanBorc = CType(Me.txtToplamBorc.Text, Single) - Odenenler

        'Borc bitip bitmedi�ini belirtme..
        If KalanBorc = 0 Then
            Me.lblBorcBitti.Text = Me.txtBorcluAdSoy.Text & " adl� ki�inin borcu kalmam��t�r."
        ElseIf KalanBorc < 0 Then
            Me.lblBorcBitti.Text = Me.txtBorcluAdSoy.Text & " adl� ki�inin borcundan fazla �demi�tir."
        Else
            Me.lblBorcBitti.Text = Me.txtBorcluAdSoy.Text & " adl� ki�inin borcu hala vard�r."
        End If

        Me.txtKalanBorc.Text = KalanBorc
    End Sub

    'Odenen borclar� database e kaydetmek i�in uygun hale getirme..
    Private Sub SetOdenenBorc()
        OdenenBorclar = Nothing

        If Not Me.lstOdenenler.Items.Count = 0 Then
            For i As Integer = 0 To Me.lstOdenenler.Items.Count - 1
                OdenenBorclar += "/" & CStr(Me.lstOdenenler.Items(i))
            Next
        Else
            OdenenBorclar = 0
        End If
    End Sub

    'Var olan borclu kayd�n� d�zenler..
    Private Sub SaveBorclu()
        Dim objListBoxNesnesi2 As New ListBoxNesnesi2
        objListBoxNesnesi2 = CType(Me.lstBorclar.SelectedItem, ListBoxNesnesi2)

        Dim UpdateQuery As OleDbQueryEvents.OleDbUpdateQuery
        UpdateQuery = New OleDbQueryEvents.OleDbUpdateQuery(Module1.strConnStr, "tblBorclar", "BorcluAdSoy", "ToplamBorc", "OdenenBorc", Me.txtBorcluAdSoy.Text, CType(Me.txtToplamBorc.Text, Single), OdenenBorclar, "BorcluID", objListBoxNesnesi2.ID)
    End Sub

    ''Var olan borclu kayd�n� d�zenler..
    'Private Sub SaveBorclu(ByVal EklenenBorc As Single)
    '    Dim objListBoxNesnesi2 As New ListBoxNesnesi2
    '    objListBoxNesnesi2 = CType(Me.lstBorclar.SelectedItem, ListBoxNesnesi2)

    '    Dim UpdateQuery As OleDbQueryEvents.OleDbUpdateQuery
    '    UpdateQuery = New OleDbQueryEvents.OleDbUpdateQuery(Module1.strConnStr, "tblBorclar", "BorcluAdSoy", "ToplamBorc", "OdenenBorc", Me.txtBorcluAdSoy.Text, CType(Me.txtToplamBorc.Text, Single), OdenenBorclar, "BorcluID", objListBoxNesnesi2.ID)
    'End Sub

    'Yeni borclu ekler..
    Private Sub NewBorclu()
        Dim TplBorc As Single = CType(Me.txtToplamBorc.Text, Single)

        SetOdenenBorc()

        Dim InsertQuery As OleDbQueryEvents.OleDbInsertQuery
        InsertQuery = New OleDbQueryEvents.OleDbInsertQuery(Module1.strConnStr, "tblBorclar", "BorcluAdSoy", "ToplamBorc", "OdenenBorc", Me.txtBorcluAdSoy.Text, TplBorc, OdenenBorclar)
    End Sub

    'Se�ili borcluyu siler..
    Private Sub DelBorclu()
        Dim objListBoxNesnesi2 As New ListBoxNesnesi2
        objListBoxNesnesi2 = CType(Me.lstBorclar.SelectedItem, ListBoxNesnesi2)

        Dim DeleteQuery As OleDbQueryEvents.OleDbDeleteQuery
        DeleteQuery = New OleDbQueryEvents.OleDbDeleteQuery(Module1.strConnStr, "tblBorclar", "BorcluID", objListBoxNesnesi2.ID)
    End Sub

    'Tum borclular� siler..
    Private Sub DelBorclular()
        Dim DeleteQuery As OleDbQueryEvents.OleDbDeleteQuery
        DeleteQuery = New OleDbQueryEvents.OleDbDeleteQuery(Module1.strConnStr, "tblBorclar")
    End Sub
#End Region

#Region "Olaylar"
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not Me.lstBorclar.SelectedIndex = -1 Then
            If Not Me.txtBorcluAdSoy.Text = Nothing Then
                SetOdenenBorc()
                SaveBorclu()
            Else
                MessageBox.Show("L�tfen Bor�lu ismini giriniz..", "Bor�lu Ismi Yok!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Exit Sub
            End If
        Else
            MessageBox.Show("L�tfen listeden bir bor�lu se�iniz..", "Se�ili Bor�lu Yok!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim User As String = InputBox("Borcu olan ki�inin ismini giriniz..", "Yeni Bor�lu Ekle")
        Dim Deger As Object
        Dim Borc As Single = 0

        If Not Trim(User.Length) = 0 Then
            Do
                Deger = InputBox(User & " adl� ki�inin borcunu giriniz..", "Bor� Miktar�")
            Loop While (IsNumeric(Deger) = False)

            Borc = Convert.ToSingle(Deger)
            Me.txtToplamBorc.Text = Borc
            Me.txtBorcluAdSoy.Text = User
            SetOdenenBorc()
            NewBorclu()
            GetBorclular()

            For i As Integer = 0 To Me.lstBorclar.Items.Count - 1
                If Me.lstBorclar.Items(i).ToString = User Then
                    Me.lstBorclar.SelectedIndex = i
                    Exit For
                End If
            Next
        Else
            MessageBox.Show("L�tfen bor�lu ismini giriniz..", "Bor�lu Ismi Yok!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        GetBorclular()
        Me.txtBorcluAdSoy.Text = Nothing
        Me.txtKalanBorc.Text = Nothing
        Me.txtToplamBorc.Text = Nothing
        Me.lblBorcBitti.Text = Nothing
        Me.lstOdenenler.Items.Clear()
    End Sub

    Private Sub btnDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDel.Click

        If Not Me.lstBorclar.SelectedIndex = -1 Then
            Dim objListBoxNesnesi2 As New ListBoxNesnesi2
            objListBoxNesnesi2 = CType(Me.lstBorclar.SelectedItem, ListBoxNesnesi2)

            DialogResult = MessageBox.Show(objListBoxNesnesi2.Value & " adl� ki�iyi silmek istedi�inize emin misniz?", "Bor�lu Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

            If DialogResult = Windows.Forms.DialogResult.Yes Then
                DelBorclu()
            Else
                Exit Sub
            End If
        Else
            MessageBox.Show("L�tfen silinmesi i�in listeden bir bor�lu se�iniz..", "Se�ili Bor�lu Yok!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub cmnuBorclular_Yeni_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuBorclular_Yeni.Click
        btnAdd_Click(sender, e)
    End Sub

    Private Sub cmnuBorclular_Kaydet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuBorclular_Kaydet.Click
        btnSave_Click(sender, e)
    End Sub

    Private Sub cmnuBorclular_Iptal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuBorclular_Iptal.Click
        btnCancel_Click(sender, e)
    End Sub

    Private Sub cmnuBorclular_Sil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuBorclular_Sil.Click
        btnDel_Click(sender, e)
    End Sub

    Private Sub cmnuBorclular_HepsiSil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuBorclular_HepsiSil.Click
        DialogResult = MessageBox.Show("T�m kay�tlar�n silinsin mi?", "T�m Kay�tlar� Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If DialogResult = Windows.Forms.DialogResult.Yes Then
            DelBorclular()
        End If
    End Sub

    Private Sub cmnuOdenen_Ekle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuOdenen_Ekle.Click
        BorcEkle = Nothing
        BorcEkle = InputBox("�denen bor� miktar�n� giriniz." & vbCrLf & "�rn:123,45", "�denen Bor� Ekle")

        If IsNumeric(BorcEkle) = True Then
            Me.lstOdenenler.Items.Add(BorcEkle)
        End If

        SetToplam()
    End Sub

    Private Sub cmnuOdenen_Sil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuOdenen_Sil.Click
        If Not Me.lstOdenenler.SelectedIndex = -1 Then
            Me.lstOdenenler.Items.RemoveAt(Me.lstOdenenler.SelectedIndex)
        Else
            MessageBox.Show("Silmek i�in bir �deme se�iniz.", "�deme Sil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

        SetToplam()
    End Sub

    'Private Sub cmnuOdenen_HepsiSil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    DialogResult = MessageBox.Show("T�m �demeler silinsin mi?", "T�m �demeleri Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

    '    If DialogResult = Windows.Forms.DialogResult.Yes Then
    '        Me.lstOdenenler.Items.Clear()
    '    Else
    '        Exit Sub
    '    End If

    '    SetToplam()
    'End Sub

    Private Sub lstOdenenler_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstOdenenler.KeyDown
        If e.KeyCode = Keys.Delete Then
            cmnuOdenen_Sil_Click(sender, e)
        End If
    End Sub

    Private Sub frmBorclar_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetBorclular()
    End Sub

    Private Sub lstBorclar_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstBorclar.SelectedIndexChanged
        GetBorcDetails()
        GetOdenenBorc()
        SetToplam()
    End Sub

    Private Sub cmnuOdenen_BorcEkle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuOdenen_BorcEkle.Click
        Dim BorcEkle As Single

        BorcEkle = InputBox("Eklenecek Borcu Giriniz." & vbCrLf & "�rn:123,56", "Bor� Ekle")

        If IsNumeric(BorcEkle) = True Then
            If Not Me.txtToplamBorc.Text = Nothing Then
                BorcEkle += CType(Me.txtToplamBorc.Text, Single)
                Me.txtToplamBorc.Text = BorcEkle
            Else
                Me.txtToplamBorc.Text = BorcEkle
            End If

            SetToplam()
        End If
    End Sub
#End Region

    
End Class