var FlickrAPI = (function (self) {
    var url = "https://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=081d1900583520a98a04fa6e4485dfaf&tags={0}&format=json&nojsoncallback=1";
    var imagesUrl = "https://farm{farm-id}.staticflickr.com/{server-id}/{id}_{secret}.jpg";

    self.getImages = function (tag) {
        var link = url.replace("{0}", tag);
        return $.get(link);
    };

    self.getImageUrl = function (photo) {
        var link = imagesUrl.replace("{farm-id}", photo.farm).replace("{server-id}", photo.server).replace("{id}", photo.id).replace("{secret}", photo.secret);
        return link;
    };

    return self;
})(FlickrAPI || {});