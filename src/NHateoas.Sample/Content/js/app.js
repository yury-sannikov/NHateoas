'use strict';

var nhateoasSampleApp = angular.module('nhateoasSampleApp', ['ng', 'ngRoute', 'ngResource', 'hateoas'])
    .config(function ($routeProvider, $locationProvider, HateoasInterceptorProvider) {
        $routeProvider.otherwise(
            {
                templateUrl: '/content/templates/ApiDiscovery.html',
                controller: 'ApiDiscoveryController'
            });
        $locationProvider.html5Mode(true);
        HateoasInterceptorProvider.transformAllResponses();
    });
