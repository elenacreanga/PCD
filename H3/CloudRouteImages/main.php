<?php
    use google\appengine\api\users\User;
    use google\appengine\api\users\UserService;
    $user = UserService::getCurrentUser();
    if (!$user) {
        header('Location: ' . UserService::createLoginURL($_SERVER['REQUEST_URI']));
        return;
    }

    $id = $user->getUserId();

    $memcache = new Memcache;

    function getData($key) {
        global $memcache;
        $data = $memcache->get($key);
        if($data === false){
            $data = [];
        }
        return $data;
    }

    $searchHistoryList = getData($id);
?>
<!DOCTYPE html>

<html>
    <head>
        <meta name="viewport" content="width=device-width" />
        <title>Route Images</title>
        <link href="stylesheets/style.css" rel="stylesheet" />
    </head>
    <body>
        <div id="form">
            <label>From:</label>
            <input type="text" id="fromInput"/>
            <label>To:</label>
            <input type="text" id="toInput" />
            <button id="btnSearch">Search</button>
            <button id="btnSendMail">Send Route Images Mail</button>
        </div>
        <div id="userHolder">
            <?php
                echo 'Hello, <b>' . htmlspecialchars($user->getNickname()) . '</b>,&nbsp';
                echo '<a href="' . UserService::createLogoutURL($_SERVER['REQUEST_URI']) . '">logout</a>';
            ?>
        </div>
        <div id="searchHistoryContainer">
            <div>
                <label>
                    <b>Search History</b>
                </label>
                <button id="btnClearSearchHistory">Clear history</button>
            </div>

            <ul>
                <?php
                    foreach ($searchHistoryList as $value) {
                        echo '<li>';
                        echo $value->from . ' -> ';
                        echo $value->to;
                        echo '</li>';
                    }
                ?>
            </ul>
        </div>
        <div id="mapContainer"></div>
        <div id="directionsContainer"></div>
        <ul id="routeImagesContainer"></ul>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
        <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&signed_in=true"></script>
        <script src="scripts/GeocodingAPI.js"></script>
        <script src="scripts/DirectionsAPI.js"></script>
        <script src="scripts/FlickrAPI.js"></script>
        <script src="scripts/History.js"></script>
        <script src="scripts/Mail.js"></script>
        <script src="scripts/script.js"></script>
    </body>
</html>
