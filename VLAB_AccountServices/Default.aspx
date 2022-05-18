<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VLAB_AccountServices.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            -receive UH username here<br />
            -do a test here to see if account exists in AD<br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;yes? redir to /services/resetPassword.aspx<br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;no? redir to /services/createAccount.aspx
        </div>
    </form>
</body>
</html>
