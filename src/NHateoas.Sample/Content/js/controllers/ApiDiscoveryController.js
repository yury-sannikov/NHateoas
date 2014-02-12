'use strict';

nhateoasSampleApp.controller('ApiDiscoveryController',
    function EventListController($scope, $location, $route, apiData) {

        if ($location.url() == '/') {
            $scope.events = apiData.getAllEvents();
        }


        $scope.nonFunctions = function(items) {
            var result = {};
            angular.forEach(items, function (value, key) {
                if (typeof value !== 'function') {
                    result[key] = value;
                }
            });
            return result;
        };

        $scope.navigate = function(event, rel) {


            var lastRoute = $route.current;
            $scope.$on('$locationChangeSuccess', function (event) {
                $route.current = lastRoute;
            });

            var href = event.queryLinks()[rel].href;
            $location.url(href);

            var methodName = rel.replace(/-/g, "_");
            $scope.events = event[methodName]();
        };
        
        $scope.links = function(event) {
            return event.queryLinks();
        };

        $scope.arrayOfEvents = function() {
            if (angular.isArray($scope.events))
                return $scope.events;

            return [$scope.events];
        };

        $scope.submit = function(event, action) {
            var json = {};
            for (var af in action.fields)
                json[action.fields[af].name] = $("#" + action.name + action.fields[af].name).val();

            var actionFn = event[action.name.replace(/-/g, "_")];

            var promice = actionFn(json);

            promice.then(function() {
                alert("Success");
            }, function(reason) {
                alert("Error: " + reason.data.MessageDetail);
            }, function() {
                alert("Notify");
            });

            return false;
        };

        $scope.hasKeys = function(obj) {
            if (!obj)
                return false;
            return Object.keys(obj).length > 0;
        };
    }
);

