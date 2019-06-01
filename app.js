var express = require('express');
var app = express();

app.get('/', function(req, res) {
    res.send('Hello Gurpreet Deol!');
});

app.listen(3000, function() {
    console.log('Test application is running on port 3000!');
});