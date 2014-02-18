'use strict';

nhateoasSampleApp.controller('ActionController',
    function ActionController($scope, $location, $route, apiData, $window) {

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



            var successFn = function success(data, status, headers, config) {
                var result = JSON.stringify(status(), undefined, 2);
                result += "\n\n";
                result += JSON.stringify(data, undefined, 2);
                $scope.responseSuccess = result;
            };
            var errorFn = function error(data, status, headers, config) {
                var result = JSON.stringify(status(), undefined, 2);
                result += "\n\n";
                result += JSON.stringify(data, undefined, 2);
                $scope.responseError = result;
            };

            var isArray = ($scope.params["class[]"] || new Array()).indexOf("__query") !== -1;

            apiData.doAction(json, $scope.params.href, isArray, $scope.params.method, successFn, errorFn);

            return false;
        };
        $scope.goBack = function () {
            $window.history.back();
        };
    }
);

