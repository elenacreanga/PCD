// composer autoloading
require_once __DIR__ . '/vendor/autoload.php';

// grab the first argument
if (empty($argv[1])) {
    die("usage: php listBuckets [project_id]\n");
}

$projectId = $argv[1];

// Authenticate your API Client
$client = new Google_Client();
$client->useApplicationDefaultCredentials();
$client->addScope(Google_Service_Storage::DEVSTORAGE_FULL_CONTROL);

$storage = new Google_Service_Storage($client);

/**
 * Google Cloud Storage API request to retrieve the list of buckets in your project.
 */
$buckets = $storage->buckets->listBuckets($projectId);

foreach ($buckets['items'] as $bucket) {
    printf("%s\n", $bucket->getName());
}