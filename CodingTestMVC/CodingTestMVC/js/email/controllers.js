/* global angular */
(function () {
  "use strict"

  var app = angular.module('email.controllers', ['ngResource', 'ngTable'])

  app.controller('emailCtrl', ['$scope', '$resource', 'NgTableParams',
    emailController
  ])

  function emailController($scope, $resource, NgTableParams) {
    var emailApi = $resource('/email/templates')
    var sortMap = {
      '+EmailLabel': 0,
      '-EmailLabel': 1,
      '+DateUpdated': 2,
      '-DateUpdated': 3,
      '+FromAddress': 4,
      '-FromAddress': 5
    }
    var dataset = [
      { EmailLabel: '1', FromAddress: 'add1', DateUpdated: '' },
      { EmailLabel: '2', FromAddress: 'add2', DateUpdated: '' },
      { EmailLabel: '3', FromAddress: 'add3', DateUpdated: '' }
    ]

    $scope.tblEmail = new NgTableParams({}, {
      getData: function (params) {
        var query = {
          sortby: sortMap[params.orderBy()],
          pagesize: params.count(),
          page: params.page()
        }

        return emailApi.get(query).$promise.then(function (data) {
          params.total(data.total)

          return data.templates
        })
      }
      //dataset: data
    })

    $scope.testFunction = function () {
      return true;
    }
  }
})()

