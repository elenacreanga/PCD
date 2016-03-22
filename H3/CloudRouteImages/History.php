<?php
    use google\appengine\api\users\User;
    use google\appengine\api\users\UserService;
    $user = UserService::getCurrentUser();
    if (!$user) {
        header('Location: ' . UserService::createLoginURL($_SERVER['REQUEST_URI']));
        return;
    }

    $method = $_SERVER['REQUEST_METHOD'];

    if($method != 'POST' && $method != 'DELETE'){
        http_response_code(405);
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

    function setData($key, $data){
        global $memcache;
        $memcache->set($key, $data);
    }

    function saveSearch(){
        global $id;
        if (array_key_exists('data', $_POST)) {
            $data = json_decode($_POST['data']);
            if($data->from && $data->to){
                $history = getData($id);
                array_push ($history, $data);
                setData($id, $history);
                http_response_code(201);
            }
            else{
                http_response_code(400);
            }
        }
        else{
            http_response_code(400);
        }
    }

    function deleteSearch(){
        global $id;
        setData($id, []);
        http_response_code(200);
    }

    if($method == 'POST'){
        saveSearch();
    }
    else if($method == 'DELETE'){
        deleteSearch();
    }