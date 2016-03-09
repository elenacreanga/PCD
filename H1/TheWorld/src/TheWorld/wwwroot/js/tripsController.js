(function() {
    "use strict";
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;
        vm.trips = [
            {
                name: "US Trip",
                created: new Date()
            },
            {
                name: "World Trip",
                created: new Date()
            }
        ];

        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function(response) {
                angular.copy(response.data, vm.trips);
            }, function(error) {
                vm.errorMessage = "Failed to load data:" + error;
            })
            .finally(function() {
                vm.isBusy = false;
            });

        vm.newTrip = {};
        vm.addTrip = function() {
            vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            vm.newTrip = {};
        };


    }
})();