using System.Web.Optimization;

namespace Lexfy.Web.Interface
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            ////ignorar carregamento
            //bundles.IgnoreList.Ignore("*.xxx.js");

            //// incluir diretório
            //bundles.Add(new ScriptBundle("~/comum")
            //    .IncludeDirectory("~/Scripts/comum", "*.js", true));

            //// ordenar chamadas de arquivo de script
            //var order = new BundleFileSetOrdering("nome");
            //order.Files.Add("sepu.js");
            //order.Files.Add("display.js");
            //bundles.FileSetOrderList.Insert(0, order);

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/vendors/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/vendors/datatables.net/css/dataTables.bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/bundles/theme/css").Include(
                      "~/Content/vendors/font-awesome/css/font-awesome.min.css",
                      "~/Content/vendors/iCheck/skins/flat/green.css",
                      "~/Content/vendors/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/Content/production/css/maps/jquery-jvectormap-2.0.3.css",
                      "~/Content/production/css/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Content/vendors/jquery/dist/jquery.min.js",
                      "~/Content/vendors/bootstrap/dist/js/bootstrap.min.js",
                      "~/Content/vendors/datatables.net/js/jquery.dataTables.min.js",
                      "~/Content/vendors/datatables.net/js/dataTables.bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/theme/js").Include(                      
                      "~/Content/vendors/jquery.validate/jquery.validate*",
                      "~/Content/vendors/modernizr/modernizr-*",
                      "~/Content/vendors/respond/respond.min.js",
                      "~/Content/vendors/fastclick/lib/fastclick.js",
                      "~/Content/vendors/nprogress/nprogress.js",
                      "~/Content/vendors/Chart.js/dist/Chart.min.js",
                      "~/Content/vendors/bernii/gauge.js/dist/gauge.min.js",
                      "~/Content/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js",
                      "~/Content/vendors/iCheck/icheck.min.js",
                      "~/Content/vendors/skycons/skycons.js",

                      // Flot
                      "~/Content/vendors/Flot/jquery.flot.js",
                      "~/Content/vendors/Flot/jquery.flot.pie.js",
                      "~/Content/vendors/Flot/jquery.flot.time.js",
                      "~/Content/vendors/Flot/jquery.flot.stack.js",
                      "~/Content/vendors/Flot/jquery.flot.resize.js",
                      "~/Content/production/js/flot/jquery.flot.orderBars.js",
                      "~/Content/production/js/flot/date.js",
                      "~/Content/production/js/flot/jquery.flot.spline.js",
                      "~/Content/production/js/flot/curvedLines.js",
                      "~/Content/production/js/maps/jquery-jvectormap-2.0.3.min",
                      "~/Content/production/js/moment/moment.min.js",
                      "~/Content/production/js/datepicker/daterangepicker.js",
                      "~/Content/production/js/custom.js"));
        }
    }
}
