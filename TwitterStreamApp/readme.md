# TwitterStream App
Basic sample app connecting to the Twitter API v2 to process the Volume Streams 1% sampled stream.
https://developer.twitter.com/en/docs/twitter-api/tweets/volume-streams/introduction

### Dependencies
* .NET Core SDK 6.0
* TwitterAPI Key
* Optional - Docker Desktop

# Twitter API Key
To obtain a Twitter API Key you will need to create/utilize a twitter account with an email address. During the application process you will also be asked to verify your phone number. You can apply here: [Twitter API Access](https://developer.twitter.com/)

# Setup/Configuration
* Go to TwitterStream.ConsoleApp\appsettings.json
* Edit the appsettings.json file and add the BearerToken value.
* Run the TwitterStream.ConsoleApp.

# Docker
A simple way to run everything in a docker container, nothing fancy.
<!-- Bash script block -->
```bash
    docker build -t twitterstreamconsoleapp -f ./TwitterStream.ConsoleApp/DOCKERFILE .
``` 
* Once the Docker image is built. run: 

<!-- Bash script block -->
```bash
    docker run twitterstreamconsoleapp
``` 