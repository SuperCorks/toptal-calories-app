#!/usr/bin/env node

/**
 * Module dependencies.
 */
const app = require('../app');
const http = require('http');

const mongoose = require('mongoose');
const MongoConfigs = require('../mongo-configs');

const AuthManager = require('calories/authentication/manager/authentication-manager');

/**
 * Get port from environment and store in Express.
 */
const port = normalizePort(process.env.PORT || '80');
app.set('port', port);

/**
 * @see https://nodejs.org/api/http.html#http_http_createserver_options_requestlistener
 *
 * @returns {Promise<Server>} A promise that resolves when the server is started and returns
 * a new instance of http(s).Server.
 */
async function start() {

    // init mongoose
    await mongoose.connect(
        `mongodb://${MongoConfigs.current.host}/${MongoConfigs.current.database}`,
        { useNewUrlParser: true }
    );

    if (!AuthManager.isInitialized) await AuthManager.initialize();

    // Create HTTP server.
    const server = http.createServer(app);

    // Listen on provided port, on all network interfaces.

    await new Promise((resolve, reject) => {

        server.listen(port);

        server.on('error', (error) => {

            if (error.syscall !== 'listen') return reject(error);

            let bind = typeof port === 'string'
                ? 'Pipe ' + port
                : 'Port ' + port;

            // handle specific listen errors with friendly messages
            switch (error.code) {
                case 'EACCES':
                    console.error(bind + ' requires elevated privileges');
                    process.exit(1);
                    break;

                case 'EADDRINUSE':
                    console.error(bind + ' is already in use');
                    process.exit(1);
                    break;

                default:
                    return reject(error);
            }
        });


        server.on('listening', () => {
            let addr = server.address();
            let bind = typeof addr === 'string'
                ? 'pipe ' + addr
                : 'port ' + addr.port;
            console.log('Listening on ' + bind);

            resolve();
        });
    });

    const oldClose = server.close;

    server.close = () => new Promise(async (resolve, reject) => {

        try {

            await mongoose.disconnect();

            oldClose.apply(server, [(e) => {
                if (e) return reject(e);
                resolve();
            }]);

        } catch (e) { reject(e) }
    });

    return server;
}

/**
 * Normalize a port into a number, string, or false.
 */
function normalizePort(val) {
    var port = parseInt(val, 10);

    if (isNaN(port)) {
        // named pipe
        return val;
    }

    if (port >= 0) {
        // port number
        return port;
    }

    return false;
}


// https://stackoverflow.com/questions/6398196/node-js-detect-if-called-through-require-or-directly-by-command-line
// If this is the main module, run automatically
if (require.main === module) {
    start().catch(console.error);
}

module.exports = {start};