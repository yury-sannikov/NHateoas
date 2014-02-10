using System.Web;
using System.Web.Optimization;

namespace NHateoas.Sample
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular/angular.js",
                        "~/Scripts/angular/angular-resource.js",
                        "~/Scripts/angular/angular-route.js",
                        "~/Scripts/angular/angular-loader.js",
                        "~/Scripts/angular/angular-hateoas.js",
                        "~/Scripts/underscore-1.4.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/content/js/app.js",
                        "~/content/js/controllers/EntityListController.js",
                        "~/content/js/controllers/MainMenuController.js",
                        "~/content/js/filters.js",
                        "~/content/js/services/EntityData.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));
        }
    }
}