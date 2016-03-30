var app = (function (self) {

    var fromInput = document.getElementById("fromInput");
    var toInput = document.getElementById("toInput");
    var btnSearch = document.getElementById("btnSearch");
    var btnSendMail = document.getElementById("btnSendMail");
    var btnClearSearchHistory = document.getElementById("btnClearSearchHistory");
    var routeImagesContainer = document.getElementById("routeImagesContainer");
    var mapContainer = document.getElementById("mapContainer");
    var searchHistoryContainer = document.getElementById("searchHistoryContainer");
    var directionsContainer = document.getElementById("directionsContainer");
    var directionsDisplay = new google.maps.DirectionsRenderer({ draggable: true });
    var map;
    var wayPointsImages;

    btnSearch.onclick = onBtnSearchClick;
    btnClearSearchHistory.onclick = onBtnClearSearchHistoryClick;
    btnSendMail.onclick = onBtnSendMailClick;

    //--events--\\
    function onBtnSearchClick() {
        var fromLocation =fromInput.value;
        var toLocation =toInput.value;
        if (isEmpty(fromLocation)) {
            alert("Insert [From] Location!");
            return;
        }
        if (isEmpty(toLocation)) {
            alert("Insert [To] Location!");
            return;
        }
        //document.getElementById("bigQueryList").innerHTML = "";
       // runQuery(fromLocation);
       // runQuery(toLocation);
        getRoute(fromLocation, toLocation);
        addRouteToHistory(fromLocation, toLocation);      
    };

    function onBtnClearSearchHistoryClick(){
        clearSearchHistory();
    };

    function onBtnSendMailClick(){
        var fromLocation = fromInput.value;
        var toLocation = toInput.value;
        if (isEmpty(fromLocation)) {
            alert("Insert [From] Location!");
            return;
        }
        if (isEmpty(toLocation)) {
            alert("Insert [To] Location!");
            return;
        }
        var body = getRouteHTML();
        sendMail(fromLocation + ' -> ' + toLocation, body);
    };
    //--events--\\

    //--methods--\\
    function initializeMap() {
        var mapOptions = {
            zoom: 7,
            center: new google.maps.LatLng(47.1561373, 27.5869704)
        };
        map = new google.maps.Map(mapContainer, mapOptions);
        directionsDisplay.setMap(map);
        directionsDisplay.setPanel(directionsContainer);
    };

    function getRoute(fromLocation, toLocation) {
        GeocodingAPI.getGeographicCoordinates(fromLocation).done(function (fromResponse) {
            if (fromResponse.status != "OK") {
                alert("Invalid [from] location!");
                return;
            }
            var fromCoordinates = {};
            fromCoordinates.lat = fromResponse.results[0].geometry.location.lat;
            fromCoordinates.lng = fromResponse.results[0].geometry.location.lng;

            GeocodingAPI.getGeographicCoordinates(toLocation).done(function (toResponse) {
                if (toResponse.status != "OK") {
                    alert("Invalid [to] location!");
                    return;
                }
                var toCoordinates = {};
                toCoordinates.lat = toResponse.results[0].geometry.location.lat;
                toCoordinates.lng = toResponse.results[0].geometry.location.lng;
                DirectionsAPI.getDirections(
                    fromResponse.results[0].geometry.location.lat,
                    fromResponse.results[0].geometry.location.lng,
                    toResponse.results[0].geometry.location.lat,
                    toResponse.results[0].geometry.location.lng,
                    directionsLoaded
                    );
            });
        });
    };

    function directionsLoaded(response, status) {
        if (status != google.maps.DirectionsStatus.OK) {
            alert("Can't find a route! + " + status);
            return;
        }
        directionsDisplay.setDirections(response);
        getImages(response);   

    };

    function getImages(route) {
        wayPointsImages = [];
        routeImagesContainer.innerHTML = "";
        for (var i = 0; i < route.routes.length; i++) {
            var steps = route.routes[i].legs[0].steps;
            for (var j = 0; j < steps.length; j++) {
                var locationLatLng = steps[j].end_location;
                GeocodingAPI.getAddress(locationLatLng.lat(), locationLatLng.lng()).done(addressLoaded);
            }
        }

    };

    function addressLoaded(response) {
    
        for (var i = 0; i < response.results.length; i++) {
            var address = response.results[i].formatted_address;
          
            if (!wayPointsImages[address]) {
                wayPointsImages[address] = true;
                FlickrAPI.getImages(address).done(onAddressImageLoaded(address));
            }
        }
       
       
    };

    function onAddressImageLoaded(address) {
        return function (response) {
            var index = Math.floor((Math.random() * (response.photos.photo.length - 1)));
            var photo = response.photos.photo[index];
            wayPointsImages[address] = FlickrAPI.getImageUrl(photo);
            var li = document.createElement("li");
            var img = document.createElement("img");
            img.src = wayPointsImages[address];
            img.alt = address;
            img.title = address;
            li.appendChild(img);
            routeImagesContainer.appendChild(li);
           
           // console.log(address + " -> " + FlickrAPI.getImageUrl(photo));
        };
    };

    function addRouteToHistory(fromLocation, toLocation){
        History.saveSearch(fromLocation, toLocation).done(function(response){
                var li = document.createElement("li");
                li.innerHTML = fromLocation + ' -> ' + toLocation;
                var ul = searchHistoryContainer.querySelector("ul:first-of-type");
                ul.appendChild(li);
            }
        );
    };

    function clearSearchHistory(){
        History.deleteSearch().done(function(response){
                var ul = searchHistoryContainer.querySelector("ul:first-of-type");
                ul.innerHTML = '';
            }
        );
    };

    function getRouteHTML(){
        return directionsContainer.outerHTML + routeImagesContainer.outerHTML;
    };

    function sendMail(route, body){
        Mail.sendMail(route, body).done(function(response){
           alert("Sent!");
        });
    };

    function isEmpty(str) {
        return (!str || 0 === str.length);
    }

       
    //--methods--\\


    initializeMap();

    return self;
})(app || {});