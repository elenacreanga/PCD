<?php
    use google\appengine\api\users\User;
    use google\appengine\api\users\UserService;
    use google\appengine\api\cloud_storage\CloudStorageTools;

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
    
    $options = [ 'gs_bucket_name' => 'https://storage.googleapis.com/pcd-h3/' ];
    $upload_url = CloudStorageTools::createUploadUrl('/upload_handler.php', $options);
?>
<!DOCTYPE html>

<html>
    <head>
        <meta name="viewport" content="width=device-width" />
        <title>Route Images</title>
        <link href="stylesheets/style.css" rel="stylesheet" />
    </head>
    <body onload="auth()">   
        <div id="form">
            <label>From:</label>
            <input type="text" id="fromInput"/>
            <label>To:</label>
            <input type="text" id="toInput" />
            <button id="btnSearch">Search</button>
            <button id="btnSendMail">Send Route Images Mail</button>
        </div>
        <br>
        <div>
            <form action="<?php echo $upload_url?>" enctype="multipart/form-data" method="post">
                Files to upload: <br>
            <input type="file" name="uploaded_files" size="40">
            <input type="submit" value="Send">
            </form>
        </div>
        <br>
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
         <div id="bigQueryContainer">
            <div>
                <label>
                    <b>Country details:</b>
                </label>             
            </div>

            <div id="bigQueryList">
               
            </div>
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

        <script src="scripts/BigQuery.js"></script>
        <script src="https://apis.google.com/js/client.js"></script>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    </body>
</html>
