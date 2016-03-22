var DirectionsAPI = (function (self) {
    //var url = "https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&key=AIzaSyBRI0wA6dEnAC8LysWzH_nQ0B_kfxBQwBk";

    //self.getDirections = function (fromLat, fromLong, toLat, toLong) {
    //    var link = url.replace("{0}", fromLat).replace("{1}", fromLong).replace("{2}", toLat).replace("{3}", toLong);
    //    return $.get(link);
    //};

    var directionsService = new google.maps.DirectionsService();

    self.getDirections = function (fromLat, fromLong, toLat, toLong, callback) {
        var request = {
            origin: new google.maps.LatLng(fromLat, fromLong),
            destination: new google.maps.LatLng(toLat, toLong),
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.METRIC
        };

        var directionsLoaded = function (response, status) {
            this.done = function (fn) {
                fn(response, status);
            }
        };

        directionsService.route(request, function (response, status) {
            if (callback) {
                callback(response, status);
            }
        });
    };

    return self;
})(DirectionsAPI || {});