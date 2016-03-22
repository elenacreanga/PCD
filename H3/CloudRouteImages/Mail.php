<?php
    use google\appengine\api\users\User;
    use google\appengine\api\users\UserService;
    $user = UserService::getCurrentUser();
    if (!$user) {
        header('Location: ' . UserService::createLoginURL($_SERVER['REQUEST_URI']));
        return;
    }

    $method = $_SERVER['REQUEST_METHOD'];

    if($method != 'POST'){
        http_response_code(405);
        return;
    }

    use \google\appengine\api\mail\Message;

    function sendMail($subject, $body){
        global $user;
        try
        {
            $message = new Message();
            $message->setSender($user->getEmail());
            $message->addTo($user->getEmail());
            $message->setSubject('[Route Images]' . $subject);
            $message->setHtmlBody($body);
            $message->send();
            return true;
        } catch (InvalidArgumentException $e) {
            echo $e;
            return false;
        }
    }

    function processRequest(){
        if (array_key_exists('data', $_POST)) {
            $data = json_decode($_POST['data']);
            if($data->body && $data->route){
                if(sendMail($data->route, $data->body)){
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
        else{
            http_response_code(400);
        }
    }

    processRequest();