'use strict';

var nhateoasSampleApp = angular.module('nhateoasSampleApp', ['ng', 'ngRoute', 'ngResource', 'hateoas'])
    .config(function ($routeProvider, $locationProvider, HateoasInterceptorProvider) {
        /*$routeProvider.when('/newEvent',
            {
                templateUrl:'content/templates/NewEvent.html',
                controller: 'EditEventController'
            });*/
        $routeProvider.when('/entities',
            {
                templateUrl: 'content/templates/EntityList.html',
                controller: 'EntityListController'
            });
        /*$routeProvider.when('/event/:eventId',
            {
                templateUrl: 'content/templates/EventDetails.html',
                controller: 'EventController',
                resolve: {
                    event: function($q, $route, eventData) {
                        var deferred = $q.defer();
                        eventData.getEvent($route.current.pathParams.eventId)
                            .then(function(event) { deferred.resolve(event); });
                        return deferred.promise;
                    }
                }
            });*/
        $routeProvider.otherwise({redirectTo: '/entities'});
        $locationProvider.html5Mode(true);
        HateoasInterceptorProvider.transformAllResponses();
    });
