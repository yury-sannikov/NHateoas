'use strict';

nhateoasSampleApp.controller('ActionController',
    function ActionController($scope, $location, $route, apiData) {

        var decodeURIStr = function (param) {
            return param.replace(/\+/g, " ");
        };

        var unwrapParams = function(params) {
            for (var param in params) {
                var match = param.match(/(?:(?:\[)([^\]]+)(?:\])(?:\[)([^\]]+)(?:\]))/);
                if (match) {
                    var index = parseInt(match[1]);
                    if (!params.fieldsData) params.fieldsData = [];
                    var arr = params.fieldsData[index];
                    var obj = arr ? arr : {};
                    obj[match[2]] = decodeURIStr(params[param]);
                    if (!arr) params.fieldsData.push(obj);
                } else {
                    params[param] = decodeURIStr(params[param]);
                }
            }
            return params;
        };

        $scope.params = unwrapParams($route.current.params);

        $scope.submit = function() {
            var json = {};
            var data = $scope.params.fieldsData;
            for (var index in data)
                json[data[index].name] = $("#" + $scope.params.name + data[index].name).val();

            var promice = apiData.doAction(json, $scope.params.href, $scope.params.method);

            promice.then(function() {
                alert("Success");
            }, function(reason) {
                alert("Error: " + reason.data.MessageDetail);
            }, function() {
                alert("Notify");
            });

            return false;
        };
    }
);

