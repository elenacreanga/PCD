var GeocodingAPI = (function (self) {
    var addressUrl = "http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=true";
    var coordinatesUrl = "http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&sensor=true";

    self.getGeographicCoordinates = function (address) {
        var link = addressUrl.replace("{0}", address);
        return $.get(link);
    };

    self.getAddress = function (lat, long) {
        var link = coordinatesUrl.replace("{0}", lat).replace("{1}", long);
        return $.get(link);
    };

    return self;
})(GeocodingAPI || {});