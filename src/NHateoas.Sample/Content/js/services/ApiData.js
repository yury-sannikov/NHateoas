
nhateoasSampleApp.factory('apiData', function ($resource, $q, $timeout) {
    var resource = $resource('/api/product/:id', {id: '@id'});
    return {
        getAllEvents: function() {
            return resource.query();
        }
    };
});
