var Mail = (function (self) {
    var addressUrl = "http://pcd-homework-h3.appspot.com/Mail.php";

    self.sendMail = function (route, body) {
        return $.post(addressUrl, {data : JSON.stringify({ route: route, body: body })} );
    };

    return self;
})(Mail || {});