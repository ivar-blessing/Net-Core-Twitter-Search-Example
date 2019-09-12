<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Twitter.Search.App.Webforms.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="twitter-feed"></div>
    </form>

    <!-- required by the script -->
    <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-3.3.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

    <!-- configure script -->
    <script>
        var hashTag = "Scotland";
    </script>

    <!-- call script -->
    <script src="/Scripts/TwitterFeed.js"></script>
</body>
</html>
