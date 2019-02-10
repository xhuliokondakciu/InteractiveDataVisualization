using System.Web;
using System.Web.Optimization;

namespace KGTMachineLearningWeb
{
    public class 
        BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                        "~/Scripts/jquery.signalR-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/charts").IncludeDirectory(
                "~/Scripts/charts", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
               "~/Scripts/codemirror/lib/codemirror.js")
               .Include("~/Scripts/codemirror/mode/xml/xml.js")
               .Include("~/Scripts/vkbeautify.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").IncludeDirectory(
                "~/Scripts/common", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancytree").Include(
                "~/Scripts/jquery.fancytree-all.js"));

            bundles.Add(new ScriptBundle("~/bundles/workspace").IncludeDirectory(
                "~/Scripts/workspace","*.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/admin")
                .IncludeDirectory(
               "~/Scripts/admin", "*.js",true
               ));

            bundles.Add(new ScriptBundle("~/bundles/contextMenu").Include(
                "~/Scripts/jquery.contextMenu.js",
                "~/Scripts/jquery.ui.position.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/ColReorderWithResize.js",
                "~/Scripts/DataTables/dataTables.fixedHeader.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.css",
                      "~/Content/fancytree-custom.css",
                      "~/Content/workspace.css",
                      "~/Content/DataTables/css/jquery.dataTables.css",
                      "~/Content/DataTables/css/responsive.dataTables.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/contextMenu").Include(
                "~/Content/jquery.contextMenu.css"));


            bundles.Add(new StyleBundle("~/Content/codemirror").Include(
                "~/Scripts/codemirror/lib/codemirror.css"));
        }
    }
}
