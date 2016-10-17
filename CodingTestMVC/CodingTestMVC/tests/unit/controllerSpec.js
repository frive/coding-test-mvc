/// <reference path="~/node_modules/karma-jasmine/lib/jasmine.js" />
"use strict";

describe('email controller unit tests', function () {
    var $scope;

    beforeEach(function () {
        module('email.controllers');
        inject(function ($rootScope) {
            $scope = $rootScope.$new();
        });
    });

    it('should have emailCtrl defined', inject(function ($controller) {
        //spec body
      var emailCtrl = $controller('emailCtrl', { $scope: $scope });
      expect(emailCtrl).toBeDefined();
      expect($scope.tblEmail).toBeDefined();

      it('should have tblEmail defined', inject(function ($controller) {
        //spec body
        var emailCtrl = $controller('emailCtrl', { $scope: $scope });
        expect($scope.tblEmail).toBeDefined();
      }));
    }));
});