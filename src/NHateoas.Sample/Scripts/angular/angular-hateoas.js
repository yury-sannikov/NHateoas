'use strict';
/**
 * @module hateoas
 *
 * An AngularJS module for working with HATEOAS.
 *
 * Setup
 * =====
 *
 * ```javascript
 * angular.module("your-application", ["hateoas"]);
 * ```
 *
 * Using HateoasInterface
 * ======================
 *
 * The `HateoasInterface` service is a class that can be instantiated to consume a raw HATEOAS response. It searches the response for a `links` property and provides a `resource` method to interact with the links.
 *
 * Assume a resource result `someResult` looks like this:
 *
 * ```json
 * {
 *     "arbitraryStringField": "some value",
 *     "arbitraryNumberField": 31,
 *     "links": [
 *         {
 *             "rel": "something-related",
 *             "href": "/arbitrary/link"
 *         },
 *         {
 *             "rel": "something-else-related",
 *             "href": "/another/arbitrary/link"
 *         }
 *     ]
 * }
 * ```
 *
 * The workflow for implementing `HateoasInterface` might look something like this:
 *
 * ```javascript
 * var someResource = $resource("/some/rest/endpoint");
 * var someResult = someResource.get(null, function () {
 *     var object = new HateoasInterface(someResult);
 *     var putResult = object.resource("something-related").put({ someData: "whatever" }, function () {
 *         // logic, etc.
 *     });
 * });
 * ```
 *
 * Using HateoasInterceptor
 * ========================
 *
 * The `HateoasInterceptor` service is a way of making your application globally HATEOAS-enabled. It adds a global HTTP response interceptor that transforms HATEOAS responses into HateoasInterface instances.
 *
 * First, initialize the interceptor:
 * 
 * ```javascript
 * app.config(function (HateoasInterceptorProvider) {
 *     HateoasInterceptorProvider.transformAllResponses();
 * });
 * ```
 *
 * Then any HATEOAS response will automatically have the `resource` method:
 * 
 * ```javascript
 * var someResource = $resource("/some/rest/endpoint");
 * var someResult = someResource.get(null, function () {
 *     var putResult = someResult.resource("something-related").put({ someData: "whatever" }, function () {
 *         // logic, etc.
 *     });
 * })
 * ```
 */
angular.module("hateoas", ["ngResource"])

	.provider("HateoasInterface", function () {

		// global Hateoas settings
		var globalHttpMethods,
			linksKey = "links",
		    actionsKey = "actions",
		    propertiesKey = "properties";

		return {

			setLinksKey: function (newLinksKey) {
				linksKey = newLinksKey || linksKey;
			},

			getLinksKey: function () {
				return linksKey;
			},

			setActionsKey: function (newActionsKey) {
			    actionsKey = newActionsKey || actionsKey;
			},

			getActionsKey: function () {
			    return actionsKey;
			},

			setHttpMethods: function (httpMethods) {
				globalHttpMethods = angular.copy(httpMethods);
			},

			$get: ["$injector", function ($injector) {

				var arrayToObject = function (keyItem, valueItem, array) {
					var obj = {};
					angular.forEach(array, function (item, index) {
						if (item[keyItem] && item[valueItem]) {
							var key = item[keyItem];
							if (angular.isArray(key))
								angular.forEach(key, function (innerKey) {
									obj[innerKey] = { href: item[valueItem], original : key };
								});
							else
								obj[item[keyItem]] = item[valueItem];
						}
					});
					return obj;
				};

			    var fixHref = function(href) {
			        if (!href)
			            return href;
			        if (href.indexOf('/') != 0)
			            return '/' + href;
			        return href;
			    };

			    var fixFunctionName = function(fn) {
			        return fn.replace(/-/g, '_');
			    };

				var fieldsChecker = function (meta, data) {
			        angular.forEach(meta.fields, function(item, index) {
			            if (typeof data[item.name] === 'undefined')
			                throw "Parameter " + item.name + " does not exists in input object";
			        });
			    };

				var actionFucntionBuilder = function (metadata) {
				    var meta = metadata;
				    var resource = $injector.get("$resource");
				    return function (query) {
				        fieldsChecker(meta, query);
				        var actionResourse = resource(fixHref(meta.href),
                            query,
				            {
				                invokeAction: {
				                    method: meta.method,
				                    isArray: meta.class.indexOf('query') !== -1
				                }
				            }
                            );
				        var result;
				        if (meta.method !== "GET")
				            result = actionResourse.invokeAction(query, query);
				        else
				            result = actionResourse.invokeAction(query);
				        return result.$promise;
				    };
			    };
                
			    var actionMetadataExtender = function (data) {
			        var fieldsArray = [];
			        if (data.fields && angular.isArray(data.fields))
			            angular.forEach(data.fields, function(item, index) {
			                fieldsArray.push(
			                    angular.extend(item, {
			                        type: item.type || 'text',
			                        value: item.value || ''
			                    }));
			            });

			        return angular.extend(data,
			        {
			            'class': data['class'] || [],
			            method: (data.method || 'GET').toUpperCase(),
			            title: data.title || data.name,
			            type: data.type || 'application/x-www-form-urlencoded',
			            fields: fieldsArray
			        });
			    };

			    var linksBuilder = function (hrefOrObject) {
			        return function () {
			            var resource = $injector.get("$resource");
			            var href, isQuery = false;
			            if (angular.isObject(hrefOrObject)) {
			                href = hrefOrObject.href;
			                isQuery = hrefOrObject.original.indexOf("query") !== -1;
			            } else {
			                href = hrefOrObject;
			            }
			            
			            return resource(fixHref(href), {})[isQuery ? "query" : "get"]();
			        };
			    };

			    var queryLinksBuilder = function (linksArray) {
			        var relLinks = arrayToObject("rel", "href", linksArray);
			        delete relLinks.query;
			        return function () {
			            return relLinks;
			        };
			    };

			    var queryActionsBuilder = function (actions) {
			        var metadata = {};
			        angular.forEach(actions, function (item, index) {
			            metadata[item.name] = actionMetadataExtender(item);
			        });
			        return function() {
			            return metadata;
			        };
			    };

			    var HateoasInterface = function (data) {

			        if (angular.isArray(data)) {
			            // recursively consume all contained arrays or objects with links
			            angular.forEach(data, function (value, key) {
			                if (key !== linksKey && angular.isObject(value) && (angular.isArray(value) || value[linksKey])) {
			                    data[key] = new HateoasInterface(value);
			                }
			            });

			            return data;
			        }
			        data = angular.extend(this, data, {});

			        data.queryLinks = queryLinksBuilder(data[linksKey]);

			        // if links are present, consume object and convert links
					if (data[linksKey]) {
					    var relLinks = arrayToObject("rel", "href", data[linksKey]);
					    for (var key in relLinks) {
					        if (key == 'query') continue;
					        data[fixFunctionName(key)] = linksBuilder(relLinks[key]);
                        }
					    delete data[linksKey];
					}

					data.queryActions = queryActionsBuilder(data[actionsKey]);

                    // Create instance method and metadata provider for each action
					if (data[actionsKey]) {
					    angular.forEach(data[actionsKey], function (item, index) {
					        data[fixFunctionName(item.name)] = actionFucntionBuilder(item);
					    });
					    delete data[actionsKey];
					}

					if (data[propertiesKey]) {
					    var props = data[propertiesKey];
					    for (var prop in props) {
					        if (data[prop])
					            throw "Response properties object contains conflicting item '" + prop + "'";
					        data[prop] = props[prop];
					    }
					    delete data[propertiesKey];
                    }

					return data;

				};

				return HateoasInterface;

			}]

		};

	})

	.provider("HateoasInterceptor", ["$httpProvider", "HateoasInterfaceProvider", function ($httpProvider, HateoasInterfaceProvider) {
		
		var linksKey = HateoasInterfaceProvider.getLinksKey();

		return {
			
			transformAllResponses: function () {
				$httpProvider.interceptors.push("HateoasInterceptor");
			},

			$get: ["HateoasInterface", "$q", function (HateoasInterface, $q) {

				return {
					response: function (response) {

						if (response && angular.isObject(response.data)) {
							response.data = new HateoasInterface(response.data);
						}

						return response || $q.when(response);

					}
				};
			}]

		};

	}]);
