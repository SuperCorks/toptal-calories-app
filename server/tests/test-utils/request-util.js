/**
 * @description Builds the options argument for the request.get/post/delete functions using a *relative url*.
 *
 * @param {string} path - The path part of the url with leading slash (e.g. /users).
 * @param {Object} [options] - Extra request options (e.g. headers, body, etc).
 *
 * @returns {Object} The options for the http client functions get, post and delete.
 *
 * @see https://github.com/request/request#requestoptions-callback
 *
 * @example
 * const mockClient = require('request');
 * const headers = {"auth-token": getToken()};
 * const body = getEntityToCommit();
 * mockClient.post(url('/path/to/my/entity', {headers, body}), (error, response, body) => {
 *      // Handle response...
 * });
 * // Will send the following request: POST http://localhost/path/to/my/entity
 */
function url(path, options) {

    let _options = {
        uri: `http://localhost${path}`,
        json: true
    };

    if (options) Object.assign(_options, options);

    return _options;
}

module.exports = {url};