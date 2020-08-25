const httpClient = require('request');
const {expect, fail} = require('test-utils/functions');
const {url} = require('test-utils/request-util');

const server = require('../../bin/server');

describe('calories/authentication/rest-api/session-endpoints', () => {

    let serverInstance;

    before(async () => {

        serverInstance = await server.start();
    });

    after(() => {
        return serverInstance.close();
    });

    context('POST /authentication/session', () => {

        it('should return 400 if the access token is not provided', async () => {

            const invalidBodies = [  // has to be an object or body-parser doesn't like it
                undefined,
                {hello: 'world'},
                {},
                {access_token: "1234"}
            ];

            for (let body of invalidBodies) {

                await new Promise((resolve, reject) => {

                    httpClient.post(url('/authentication/session', {body}), (error, response, body) => {

                        if (error) return reject(error);

                        if (response.statusCode !== 400)
                            return reject(new Error(`Wrong response code when ${JSON.stringify(body)} given as body!`));

                        resolve();
                    });
                });
            }
        });
    });
});