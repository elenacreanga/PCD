var History = (function (self) {
    var addressUrl = "http://routeimages.appspot.com/History.php";

    self.saveSearch = function (fromLocation, toLocation) {
        return $.post(addressUrl, {data : JSON.stringify({ from: fromLocation, to: toLocation })} );
    };

    self.deleteSearch = function () {
        return $.ajax({type: "DELETE", url: addressUrl});
    };

    return self;
})(History || {});