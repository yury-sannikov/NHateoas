'use strict';

nhateoasSampleApp.controller('EventController',
    function EventController($scope, $route) {
    	$scope.sortorder = 'name';
        $scope.event = $route.current.locals.event;

        $scope.upVoteSession = function(session) {
            session.upVoteCount++;
        };


        $scope.downVoteSession = function(session) {
            session.upVoteCount--;
        };
    }
);
