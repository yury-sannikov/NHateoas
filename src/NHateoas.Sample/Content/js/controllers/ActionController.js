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

        var getMethodInvoker = function (isQuery, json) {
            var href = $scope.params.href;
            var match = href.match(/:[^&]+/g);
            angular.forEach(match, function (item, index) {
                var paramName = item.substring(1, item.length);
                href = href.replace(item, json[paramName]);
            });
            if (isQuery)
                href = href.replace("?", "/__query__?");
            $location.url(href);
        };

        $scope.submit = function () {
            var json = {};
            var data = $scope.params.fieldsData;
            for (var index in data)
                json[data[index].name] = $("#" + $scope.params.name + data[index].name).val();

            var isQuery = ($scope.params["class[]"] || new Array()).indexOf("__query") !== -1;


            if ($scope.params.method.toUpperCase() == "GET") {
                getMethodInvoker(isQuery, json);
                return false;
            }

            var convertFn = function success(data, status, headers, config) {
                return JSON.stringify(status(), undefined, 2) + "\n\n" + JSON.stringify(data, undefined, 2);
            };

            var successFn = function success(data, status, headers, config) { $scope.responseSuccess = convertFn(data, status, headers, config); };
            var errorFn = function error(data, status, headers, config) { $scope.responseError = convertFn(data, status, headers, config); };

            apiData.doAction(json, $scope.params.href, isQuery, $scope.params.method, successFn, errorFn);
            return false;
        };

        $scope.goBack = function () {
            $window.history.back();
        };
    }
);

