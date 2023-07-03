using System;
using MicroPay.Web.Models;
using System.Web;
using System.Web.UI;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using Nafed.MicroPay.Common;
using CrystalDecisions.Shared;

namespace MicroPay.Web
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ReportDocument objReport = new ReportDocument();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null && Session["ReportModel"] != null)
                    InvokeReport();
                else
                {
                    var page = HttpContext.Current.Handler as Page;
                    Response.Redirect(page.GetRouteUrl("Login", new { Controller = "Login", Action = "Index" }), false);
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            InvokeReport();
        }

        private void InvokeReport()
        {
            try
            {
                var rptModel = (BaseReportModel)Session["ReportModel"];
                string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
                objReport.Load(path);
                //   objReport.DataSourceConnections.Clear();
                //objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));
                ConnectionInfo connectInfo = new ConnectionInfo()
                {
                    ServerName = ConfigManager.Value("odbcServer"),
                    DatabaseName = ConfigManager.Value("odbcDatabase"),
                    UserID = ConfigManager.Value("odbcUser"),
                    Password = ConfigManager.Value("odbcPassword")
                };
                objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"));
                foreach (Table tbl in objReport.Database.Tables)
                {
                    tbl.LogOnInfo.ConnectionInfo = connectInfo;
                    tbl.ApplyLogOnInfo(tbl.LogOnInfo);
                }

                objReport.Refresh();
                if (rptModel.reportParameters?.Count > 0)
                {
                    foreach (var item in rptModel.reportParameters)
                        objReport.SetParameterValue($"@{item.name}", item.value);
                }
                CrystalReportViewer1.CssFilename = "fff";
                CrystalReportViewer1.ReportSource = objReport;
                // CrystalReportViewer1.DataBind();
                CrystalReportViewer1.DisplayToolbar = true;
                CrystalReportViewer1.HasPrintButton = true;
                CrystalReportViewer1.HasCrystalLogo = false;
                CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                CrystalReportViewer1.BestFitPage = true;
                // CrystalReportViewer1.ID = rptModel.rptName; 

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            objReport.Close();
            objReport.Dispose();
        }
    }
}