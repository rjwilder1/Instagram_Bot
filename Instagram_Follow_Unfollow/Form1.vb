Imports System.Threading
Imports OpenQA.Selenium
Imports OpenQA.Selenium.Chrome
Public Class Form1
    Dim Count As Integer = 0
    Dim Viewing As Boolean = True
    Public Function NL(txt As String)
        TextBox3.Text += "[" + TimeOfDay.ToString("h:mm:ss tt") + "] " + txt & vbNewLine
        Return 0
    End Function
    Function Login(Yo As String)
        Label6.Text = "Running"
        CheckBox1.Enabled = False
        CheckBox2.Enabled = False
        CheckBox3.Enabled = False
        Dim UserName = "#loginForm > div > div:nth-child(1) > div > label > input"
        Dim Password = "#loginForm > div > div:nth-child(2) > div > label > input"
        Dim LogButton = "#loginForm > div > div:nth-child(3) > button > div"
        Dim SuccessLogin = "body > div.RnEpo.Yx5HN > div > div > div > div.mt3GC > button.aOOlW.HoLwm"
        Button1.Enabled = False
        Dim options = New ChromeOptions()
        options.AddArguments("--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--proxy-bypass-list=*", "--proxy-server='direct://'", "--log-level=3", "--hide-scrollbars", "--disable-extensions") 'Path To your chrome profile   "user-data-dir=C:\Users\RJ\AppData\Local\Google\Chrome\User Data", 
        Dim drivserv = ChromeDriverService.CreateDefaultService("C:\")
        drivserv.HideCommandPromptWindow = True
        Dim Browser = New ChromeDriver(drivserv, options)
        AddHandler MyBase.FormClosing, Sub(sender As Object, e As FormClosingEventArgs)
                                           Browser.Quit()
                                           Application.Exit()
                                           Me.Close()
                                       End Sub
        Browser.Manage().Window().Position = New Point(100000, 10000)
        Browser.Navigate.GoToUrl("https://instagram.com")
        Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10)
        'Logging in
        NL("Logging into " + TextBox1.Text)
        Browser.FindElementByCssSelector(UserName).SendKeys(TextBox1.Text)
        Browser.FindElementByCssSelector(Password).SendKeys(TextBox2.Text)
        Browser.FindElementByCssSelector(LogButton).Click()
        Try
            Browser.FindElementByCssSelector(SuccessLogin)
            NL("Successfully logged in.")

        Catch ex As Exception
            NL("Failed to log in. Please restart the program and try again.")
            Return 1
        End Try
        If Yo = "Unfollow" Then
            Dim Thrd = New Thread(Sub() Unfollow(Browser))
            Thrd.Start()
        ElseIf Yo = "Follow" Then
            Dim Thrd = New Thread(Sub() FollowFollowers(Browser))
            Thrd.Start()
        ElseIf Yo = "FollowLikes" Then
            Dim Thrd = New Thread(Sub() FollowLikes(Browser))
            Thrd.Start()
        End If
        'Button1.Enabled = False
        Return 0
    End Function

    Function Unfollow(Browser As OpenQA.Selenium.Chrome.ChromeDriver)
        AddHandler MyBase.FormClosing, Sub(sender As Object, e As FormClosingEventArgs)
                                           Browser.Quit()
                                           Application.Exit()
                                           Me.Close()
                                       End Sub
        Button1.Enabled = False
        If Count = 3 Then
            NL("Finished unfollowing")
            Count = 0
            Return 0
        End If
        Browser.Navigate.GoToUrl("https://instagram.com/" + TextBox1.Text)
        Dim Following = "#react-root > section > main > div > header > section > ul > li:nth-child(3) > a"
        Dim UnfollowConfirm = "body > div:nth-child(19) > div > div > div > div.mt3GC > button.aOOlW.-Cab_"
        Dim TotalFollowing = "#react-root > section > main > div > header > section > ul > li:nth-child(3) > a > span"
        Dim UnfollowNum As Integer = Int(Browser.FindElementByCssSelector(TotalFollowing).Text)
        NL("Amount to unfollow: " + Browser.FindElementByCssSelector(TotalFollowing).Text)
        Browser.FindElementByCssSelector(Following).Click()
        Thread.Sleep(1000)
        NL("Starting to unfollow " + Browser.FindElementByCssSelector(TotalFollowing).Text + " users")
        For p As Integer = 1 To UnfollowNum
            Try
                Dim rd As New Random()
                Dim RanNum As Integer = rd.Next(1000, 5000)
                Thread.Sleep(RanNum)
                Try
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/ul/div/li[" + p.ToString + "]/div/div[3]"))
                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()
                        Try
                            Browser.FindElementByCssSelector(UnfollowConfirm).Click()
                        Catch ex As Exception
                        End Try
                        Thread.Sleep(2500)
                        If i.Text = "Following" Then
                            NL("Cannot follow anymore... Sleeping for 5 minutes.")
                            For p1 As Integer = 0 To 300000
                                Thread.Sleep(1000)
                                Label6.Text = "Sleeping for " + p1.ToString() + "ms"
                            Next

                        End If
                    Next
                Catch ex As Exception
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/ul/div/li[" + p.ToString + "]/div/div[2]"))
                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()
                        Try
                            Browser.FindElementByCssSelector(UnfollowConfirm).Click()
                        Catch ex1 As Exception
                        End Try
                        Thread.Sleep(2500)
                        If i.Text = "Following" Then
                            NL("Cannot follow anymore... Sleeping for 5 minutes.")
                            For p1 As Integer = 0 To 300000
                                Thread.Sleep(1000)
                                Label6.Text = "Sleeping for " + p1.ToString() + "ms"
                            Next
                        End If
                    Next
                End Try
                NL("Unfollowed " + p.ToString + "/" + UnfollowNum.ToString())
            Catch ex As Exception
                NL("An error occured... Sleeping for 5 minutes.")
                For p1 As Integer = 0 To 300000
                    Thread.Sleep(1000)
                    Label6.Text = "Sleeping for " + p1.ToString() + "ms"
                Next
            End Try
        Next
        Count += 1
        NL("Rerunning to make sure all are unfollowed. " + Count.ToString() + "/3")
        Unfollow(Browser)
        Button1.Enabled = True
        Browser.Quit()
        CheckBox1.Enabled = True
        CheckBox2.Enabled = True
        CheckBox3.Enabled = True
        Label6.Text = "Inactive"
        Return 0
    End Function
    Function FollowFollowers(Browser As OpenQA.Selenium.Chrome.ChromeDriver)
        AddHandler MyBase.FormClosing, Sub(sender As Object, e As FormClosingEventArgs)
                                           Browser.Quit()
                                           Application.Exit()
                                           Me.Close()
                                       End Sub
        Button1.Enabled = False
        Browser.Navigate.GoToUrl("https://instagram.com/" + TextBox4.Text)
        Dim Following = "#react-root > section > main > div > header > section > ul > li:nth-child(2) > a"
        Dim TotalFollowing = "#react-root > section > main > div > header > section > ul > li:nth-child(2) > a > span"
        Dim UnfollowNum As Integer = Int(Browser.FindElementByCssSelector(TotalFollowing).GetAttribute("title"))
        NL("Amount to follow: " + Browser.FindElementByCssSelector(TotalFollowing).GetAttribute("title"))
        Browser.FindElementByCssSelector(Following).Click()
        Thread.Sleep(1000)
        NL("Starting to follow " + Browser.FindElementByCssSelector(TotalFollowing).GetAttribute("title") + " users")
        For p As Integer = 1 To UnfollowNum
            Try
                Dim rd As New Random()
                Dim RanNum As Integer = rd.Next(1000, 5000)
                Thread.Sleep(RanNum)
                Try
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/ul/div/li[" + p.ToString + "]/div/div[3]"))
                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()

                    Next
                Catch ex As Exception
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/ul/div/li[" + p.ToString + "]/div/div[2]"))
                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()
                    Next
                End Try
                NL("Followed " + p.ToString + "/" + UnfollowNum.ToString())
            Catch ex As Exception
                NL("Cannot follow anymore... Sleeping for 5 minutes.")
                For p1 As Integer = 0 To 300000
                    Thread.Sleep(1000)
                    Label6.Text = "Sleeping for " + p1.ToString() + "ms"
                Next
            End Try
        Next
        NL("Finished following " + UnfollowNum.ToString())
        Button1.Enabled = True
        Browser.Quit()
        CheckBox1.Enabled = True
        CheckBox2.Enabled = True
        CheckBox3.Enabled = True
        Label6.Text = "Inactive"
        Return 0
    End Function
    Function FollowLikes(Browser As OpenQA.Selenium.Chrome.ChromeDriver)
        AddHandler MyBase.FormClosing, Sub(sender As Object, e As FormClosingEventArgs)
                                           Browser.Quit()
                                           Application.Exit()
                                           Me.Close()
                                       End Sub
        Button1.Enabled = False
        Browser.Navigate.GoToUrl(TextBox5.Text)
        Dim Following = "#react-root > section > main > div > div.ltEKP > article > div.eo2As > section.EDfFK.ygqzn > div > div > a"
        Dim TotalFollowing = "#react-root > section > main > div > div.ltEKP > article > div.eo2As > section.EDfFK.ygqzn > div > div > a > span"
        Dim UnfollowNum As Integer = Int(Browser.FindElementByCssSelector(TotalFollowing).Text)
        NL("Amount to follow: " + Browser.FindElementByCssSelector(TotalFollowing).Text)
        Browser.FindElementByCssSelector(Following).Click()
        Thread.Sleep(1000)
        NL("Starting to follow " + Browser.FindElementByCssSelector(TotalFollowing).Text + " users")
        For p As Integer = 1 To UnfollowNum
            Try
                Dim rd As New Random()
                Dim RanNum As Integer = rd.Next(1000, 5000)
                Thread.Sleep(RanNum)
                Try
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/div/div/div[" + p.ToString() + "]/div[3]"))

                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()
                    Next
                Catch ex As Exception
                    Dim par = Browser.FindElement(By.XPath("/html/body/div[6]/div/div/div[2]/div/div/div[" + p.ToString() + "]/div[3]"))
                    Dim lis = par.FindElements(By.XPath(".//*"))
                    For Each i In lis
                        Browser.ExecuteScript("arguments[0].scrollIntoView();", i)
                        i.Click()
                    Next
                End Try
                NL("Followed " + p.ToString + "/" + UnfollowNum.ToString())
            Catch ex As Exception
                NL("Cannot follow anymore... Sleeping for 5 minutes.")
                For p1 As Integer = 0 To 300000
                    Thread.Sleep(1000)
                    Label6.Text = "Sleeping for " + p1.ToString() + "ms"
                Next
            End Try
        Next
        NL("Finished following " + UnfollowNum.ToString())
        Button1.Enabled = True
        Browser.Quit()
        CheckBox1.Enabled = True
        CheckBox2.Enabled = True
        CheckBox3.Enabled = True
        Label6.Text = "Inactive"
        Return 0
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.TextLength <= 2 Then
            MsgBox("Please input valid account details")
            Return
        End If
        If TextBox2.TextLength <= 2 Then
            MsgBox("Please input valid account details")
            Return
        End If
        If CheckBox1.Checked = True Then
            MsgBox("Please do not close the opening Chrome browser!")
            NL("Starting")
            Dim Thrd = New Thread(Sub() Login("Follow"))
            Thrd.Start()
        ElseIf CheckBox2.Checked = True Then
            MsgBox("Please do not close the opening Chrome browser!")
            NL("Starting")
            Dim Thrd = New Thread(Sub() Login("FollowLikes"))
            Thrd.Start()
        ElseIf CheckBox3.Checked = True Then
            MsgBox("Please do not close the opening Chrome browser!")
            NL("Starting")
            Dim Thrd = New Thread(Sub() Login("Unfollow"))
            Thrd.Start()
        Else
            MsgBox("Please pick an option.")
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            CheckBox1.Checked = False
            CheckBox3.Checked = False
            TextBox5.ReadOnly = False
            TextBox4.ReadOnly = True
        Else
            TextBox5.ReadOnly = True
            TextBox4.ReadOnly = True
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            CheckBox2.Checked = False
            CheckBox3.Checked = False
            TextBox4.ReadOnly = False
            TextBox5.ReadOnly = True
        Else
            TextBox5.ReadOnly = True
            TextBox4.ReadOnly = True
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            CheckBox2.Checked = False
            CheckBox1.Checked = False
            TextBox5.ReadOnly = True
            TextBox4.ReadOnly = True
        Else
            TextBox5.ReadOnly = True
            TextBox4.ReadOnly = True
        End If
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Application.Exit()
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Viewing = True Then
            TextBox2.UseSystemPasswordChar = False
            Viewing = False
        Else
            TextBox2.UseSystemPasswordChar = True
            Viewing = True
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub
End Class