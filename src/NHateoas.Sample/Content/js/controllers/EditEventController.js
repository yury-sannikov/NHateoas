'use strict';

nhateoasSampleApp.controller('EditEventController',
    function EditEventController($scope, entityData) {

        $scope.event = {};

        $scope.saveEvent = function (event, form) {
            if(form.$valid) {
                entityData.save(event);
            }
        };

        $scope.cancelEdit = function () {
            window.location = "/content/EventDetails.html";
        };

    }
);