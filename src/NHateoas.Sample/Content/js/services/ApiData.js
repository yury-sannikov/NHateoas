
nhateoasSampleApp.factory('apiData', function ($resource, $q, $timeout) {
    var resource = $resource('/api/product/:id', {id: '@id'});
    return {
        getApiByPath: function (path) {
            
            if (path == 'api')
                return resource.query();

            var isQuery = path.indexOf("/__query__") !== -1;

            if (isQuery) {
                path = path.replace("/__query__", "");
                return $resource('/' + path, {}).query();
            } else {
                return $resource('/' + path, { }).get();
            }
        },
        doAction: function (json, url, isArray, method, success, error) {
            var res = $resource('/' + url, json,
                {invokeAction: {
                    method: method,
                    isArray: isArray
                }});

            if (method.toLocaleLowerCase() == "get")
                return res.invokeAction(json, success, error);
            else
                return res.invokeAction(json, json, success, error);
        }
    };
});
