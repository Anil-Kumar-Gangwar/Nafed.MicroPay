﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="MicroPay.Web.ReportViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src='<%=ResolveUrl("~/crystalreportviewers13/js/crviewer/crv.js")%>' type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div> 
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" EnableDatabaseLogonPrompt="false"   />
    </div>
    </form>
</body>
</html>
