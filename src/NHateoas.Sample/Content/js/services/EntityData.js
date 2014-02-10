
nhateoasSampleApp.factory('entityData', function ($resource, $q, $timeout) {
    var resource = $resource('/api/product/:id', {id: '@id'});
    return {
        getEvent: function (eventId) {
            var deferred = $q.defer();
            $timeout(function() {
                resource.get({id: eventId},
                    function (event) {
                        deferred.resolve(event);
                    },
                    function (response) {
                        deferred.reject(response);
                    });
            }, 3000);
            return deferred.promise;
        },
        save: function(event) {
            var deferred = $q.defer();
            event.id = 999;
            resource.save(event,
                function(response) { deferred.resolve(response);},
                function(response) { deferred.reject(response);}
            );
            return deferred.promise;
        },
        getAllEvents: function() {
            return resource.query();
        }
    };
});
