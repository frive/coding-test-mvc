# Technologies

## Backend
* ServiceStack 4
* .NET Framework 4.5.2
* C#

## Frontend
* AngularJS 1.5.0
* Bootstrap
* JQuery
* Gulp
* Karma

## Tests
* JS

    `$ cd <project dir>\CodingTestMVC\CodingTestMVC\CodingTestMVC`
 
    `$ npm test`

## [Demo](http://codingtestmvc.apphb.com/)

## Bonus!

* Run app in non-Windows environment using [docker](https://www.docker.com/)

    `$ cd <project dir>\CodingTestMVC\CodingTestMVC\CodingTestMVC`
    
    `$ docker build -t codingtestmvc .`
    
    `$ docker run -it --rm -p 80:80 -e USERSITE=localhost -e USERLOCATION=/var/www/app codingtestmvc`
    
    Open browser and go to localhost.
  
