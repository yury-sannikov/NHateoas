'use strict';

nhateoasSampleApp.controller('ApiDiscoveryController',
    function EventListController($scope, $location, $route, apiData) {

        $scope.events = apiData.getApiByPath($route.current.params.apipath);

        $scope.makeArray = function (events) {
            if (angular.isArray(events))
                return events;

            return [events];
        };

        $scope.getProperties = function (items) {
            var result = {};
            angular.forEach(items, function (value, key) {
                if (typeof value !== 'function') {
                    result[key] = value;
                }
            });
            return result;
        };

        $scope.navigate = function(event, rel) {
            var navLink = event.queryLinks()[rel];
            var href = navLink.href + (navLink.isQuery ? "/__query__" : "");
            $location.url(href);
        };
        
        $scope.links = function(event) {
            return event.queryLinks();
        };

        $scope.hasKeys = function(obj) {
            if (!obj)
                return false;
            return Object.keys(obj).length > 0;
        };

        $scope.navigateToAction = function(event, action) {
            var actionLink = event.queryActions()[action.name];
            var href = actionLink.href + "/__action__?" + $.param(actionLink);
            $location.url(href);
        };
    }
);

