using System.Web;
using System.Web.Optimization;

namespace App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.easing.1.3.js",
                        "~/Scripts/jquery.nicescroll.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jqConfirm").Include(
                        "~/Scripts/WebTheme/jquery-confirm.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/pace").Include(
                        "~/Scripts/WebTheme/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/appCustom").Include(
                       "~/Scripts/WebTheme/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/cropping").Include(
                        "~/Scripts/WebTheme/cropping/cropper.min.js",
                        "~/Scripts/WebTheme/cropping/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/notify").Include(
                        "~/Scripts/WebTheme/notify/pnotify.core.js",
                       "~/Scripts/WebTheme/notify/pnotify.buttons.js",                       
                       "~/Scripts/WebTheme/notify/pnotify.nonblock.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/themeCss").Include(
                     "~/Content/WebTheme/custom.css",
                     "~/Content/WebTheme/icheck/flat/blue.css",
                     "~/Content/WebTheme/animate.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootsrap.checkbox.css",
                      "~/Content/WebTheme/jquery-confirm.min.css",
                      "~/Content/font-awesome.min.css"));
        }
    }
}
