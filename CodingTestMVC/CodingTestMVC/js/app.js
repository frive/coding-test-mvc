/* global angular */
(function () {
  "use strict";
  // Declare app level module which depends on filters, and services
  var module = angular.module('app', [
    'ngRoute',
    'email.controllers',
    'navigation.controllers'
  ]);

  module.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $routeProvider.when('/', { templateUrl: '/partials/email/email.html', controller: 'emailCtrl' });
    $routeProvider.when('/404', { templateUrl: '/partials/404.html' });
    $routeProvider.otherwise({ redirectTo: '/404' });

    $locationProvider.html5Mode(true);
  }]);

})();