using System.Web;
using System.Web.Optimization;

namespace MicroPay.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //    bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                 "~/Content/bootstrap.min.css", "~/Content/jquery-1.12.1-jquery-ui.css",
                "~/Content/font-awesome.min.css", "~/Content/main_styles.css"
                ));

            bundles.Add(new StyleBundle("~/fonts").Include("~/fonts/fontawesome-free-5.6.3-web/css/all.css"));

            bundles.Add(new StyleBundle("~/Content/calender").Include("~/Content/monthcalender/css/pseudo-ripple.css",
           "~/Content/monthcalender/css/nao-calendar.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/moment.js", "~/Scripts/jquery.min.js", "~/Scripts/bootbox.min.js",
                     "~/Scripts/jquery.bootstrap-growl.js", "~/Scripts/bootstrap-datetimepicker.js",
                     "~/Scripts/popper.min.js", "~/Scripts/bootstrap.min.js", "~/Scripts/ajaxSetup.js",
                     "~/Scripts/jquery.table2excel.min.js"
                     ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            //for datetime picker
            bundles.Add(new ScriptBundle("~/bundles/jqueryui-datepicker").Include(
          "~/Scripts/jquery-1.10.3-jquery-ui.js"));

            bundles.Add(new StyleBundle("~/bundles/Css-jqueryui-datepicker").Include(
         "~/Content/themes/base/jquery-1.10.3-jquery-ui.css"));



            bundles.Add(new StyleBundle("~/Content/DataTable-css").Include(
                                           "~/Content/DataTable/css/responsive.dataTables.min.css",
                                           "~/Content/DataTable/css/jquery.dataTables.min.css",
                                           "~/Content/DataTable/css/dataTables.bootstrap4.css",
                                            "~/Content/DataTable/css/buttons.dataTables.min.css"
                                        //,
                                        //"~/Content/DataTable/css/responsive.bootstrap4.css"

                                        ));

            bundles.Add(new ScriptBundle("~/Content/DataTable-js").Include(
                "~/Content/DataTable/js/jquery.dataTables.min.js",
                "~/Content/DataTable/js/dataTables.responsive.min.js"
                                        ,
                                        "~/Content/DataTable/js/jquery.dataTables.js",
                                        "~/Content/DataTable/js/dataTables.buttons.min.js",
                                        "~/Content/DataTable/js/buttons.flash.min.js",
                                        "~/Content/DataTable/js/jszip.min.js",
                                        "~/Content/DataTable/js/pdfmake.min.js",
                                        "~/Content/DataTable/js/vfs_fonts.js",
                                         "~/Content/DataTable/js/buttons.html5.min.js",
                                          "~/Content/DataTable/js/buttons.print.min.js"

                                        //"~/Content/DataTable/js/dataTables.bootstrap4.js"

                                        ));

            bundles.Add(new ScriptBundle("~/fonts/Js").Include("~/fonts/fontawesome-free-5.6.3-web/js/all.js"));
            bundles.Add(new ScriptBundle("~/Content/monthcalenderJS").Include("~/Content/monthcalender/js/pseudo-ripple.js", "~/Content/monthcalender/js/nao-calendar.js"));

            //#if DEBUG
            //  BundleTable.EnableOptimizations = false;
            //#else
            //   BundleTable.EnableOptimizations = true;
            //#endif


            BundleTable.EnableOptimizations = true;

        }
    }
}
