const express = require('express');
const serveStatic = require("serve-static")
const path = require('path');
const history = require('connect-history-api-fallback');
const dotenv = require('dotenv').config();

let app = express();
app.use(history());
app.use(serveStatic(path.join(__dirname, 'dist')));
const port = process.env.PORT || 80;

let listener = app.listen(port, function(){
    console.log('Listening on port ' + listener.address().port); 
});
