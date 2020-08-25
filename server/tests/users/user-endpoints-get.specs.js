const httpClient = require('request');
const {expect, fail, assert} = require('test-utils/functions');
const {url} = require('test-utils/request-util');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const server = require('../../bin/server');

const MongoConfigs = require('../../mongo-configs');
const User = require('calories/users/entities/user');
const UserDao = require('calories/users/daos/user-dao');
const {UserModel} = require('calories/users/storage/mongoose-user');

const SearchUsersEndpoint = "/users";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const userDao = new UserDao();

describe('calories/users/rest-api/user-endpoints-get GET', () => {

    let serverInstance;

    before(async () => {

        AuthManager.testUser = new User({id: 'a1', role: 'admin'});

        MongoConfigs.current = TestDatabaseConfigs;

        serverInstance = await server.start();
    });

    after(() => {
        AuthManager.testUser = undefined;

        MongoConfigs.reset();
        return serverInstance.close();
    });

    context('GET /users', () => {

        let allUsers = undefined;

        // Seed users for tests of "GET /users".
        before(async () => {
            allUsers = (await Promise.all([
                userDao.commit(new User({username: 'member_1@calories.com', role: 'member', settings: {dailyCalories: 100}})),
                userDao.commit(new User({username: 'manager_1@calories.com', role: 'manager', settings: {dailyCalories: 100}})),
                userDao.commit(new User({username: 'admin_1@calories.com', role: 'admin', settings: {dailyCalories: 100}})),
                userDao.commit(new User({username: 'member_2@calories.com', role: 'member', settings: {dailyCalories: 100}})),
            ]));
        });

        // After tests of "GET /users", remove all users from test database.
        after(() => UserModel.deleteMany());

        it('should return all users when no params', async () => {

            await new Promise((resolve, reject) => {

                httpClient.get(url(SearchUsersEndpoint), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        function byId(a, b) { return a < b ? -1 : 1; }

                        expect(body.sort(byId)).to.eql(allUsers.sort(byId));

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });

        it('should filter users based on the roles query param', async () => {

            let rolesToTry = [
                [],
                ['admin'                       ],
                ['admin',   'member'           ],
                ['manager', 'admin'            ],
                ['admin',   'manager'          ],
                ['admin',   'member', 'manager'],
                ['manager', 'member', 'admin'  ],
            ];

            function byId(a, b) { return a < b ? -1 : 1; }

            for (let roles of rolesToTry) {

                await new Promise((resolve, reject) => {

                    httpClient.get(url(`${SearchUsersEndpoint}?roles=${roles.join(',')}`), (error, response, body) => {

                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code when ${JSON.stringify(body)} given as body!`));

                        try {

                            let expectedUsers = allUsers.filter(u => roles.indexOf(u.role) !== -1);

                            expect(
                                body.sort(byId),
                                `Didn't return the right users when the roles where: [${roles.join(',')}]`
                            ).to.eql(expectedUsers.sort(byId));

                            resolve();
                        } catch (e) {
                            reject(e);
                        }
                    });
                });
            }
        });
    });
});