(function() {
    "use strict";
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;
        vm.trips = [];
        vm.newTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;
        var url = "/api/trips";

        $http.get(url)
            .then(function(response) {
                angular.copy(response.data, vm.trips);
            }, function(error) {
                vm.errorMessage = "Failed to load data:" + error;
            })
            .finally(function() {
                vm.isBusy = false;
            });

        vm.addTrip = function() {

            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post(url, vm.newTrip)
                .then(function(response) {
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function(error) {
                    vm.errorMessage = "Failed to save new trip:" + error;
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };


    }
})();