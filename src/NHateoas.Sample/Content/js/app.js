'use strict';

var nhateoasSampleApp = angular.module('nhateoasSampleApp', ['ng', 'ngRoute', 'ngResource', 'hateoas'])
    .config(function ($routeProvider, $locationProvider, HateoasInterceptorProvider) {

        $routeProvider.when("/:apipath*/__action__", {
            templateUrl: '/content/templates/Action.html',
            controller: 'ActionController'
        });

        $routeProvider.when("/:apipath*", {
            templateUrl: '/content/templates/ApiDiscovery.html',
            controller: 'ApiDiscoveryController'
        });

        $routeProvider.otherwise({ redirectTo: '/api' });
        $locationProvider.html5Mode(true);
        HateoasInterceptorProvider.transformAllResponses();
    });
