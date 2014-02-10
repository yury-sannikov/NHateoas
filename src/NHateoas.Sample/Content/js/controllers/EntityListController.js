'use strict';

nhateoasSampleApp.controller('EntityListController',
    function EventListController($scope, $location, entityData) {
        $scope.events = entityData.getAllEvents();

        $scope.nonFunctions = function(items) {
            var result = {};
            angular.forEach(items, function(value, key) {
                if (typeof value !== 'function' && key !== "links" && key !== "actions") {
                    result[key] = value;
                }
            });
            return result;
        };

        $scope.navigate = function(event, rel) {

            $scope.events = [event.resource(rel)];
        };
    }
);

