const httpClient = require('request');
const {expect, fail, assert} = require('test-utils/functions');
const {url} = require('test-utils/request-util');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const server = require('../../bin/server');

const MongoConfigs = require('../../mongo-configs');
const User = require('calories/users/entities/user');
const UserDao = require('calories/users/daos/user-dao');
const {UserModel} = require('calories/users/storage/mongoose-user');

const DeleteUserEndpoint = "/users";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const userDao = new UserDao();

describe('calories/users/rest-api/user-endpoints DELETE', () => {

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

    context('DELETE /users', () => {

        // After tests of "DELETE /users", remove all users from test database.
        after(() => UserModel.deleteMany());

        it('should delete a user that is in the database', async () => {

            // Clear database
            await UserModel.deleteMany();

            let userToDelete = await userDao.commit(new User({
                username: "test-users@example.com",
                role: 'member',
                settings: {dailyCalories: 100}
            }));

            await new Promise((resolve, reject) => {

                httpClient.delete(url(`${DeleteUserEndpoint}/${userToDelete.id}`), (error, response, body) => {

                    if (error) return reject(error);

                    if (response.statusCode !== 200)
                        return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                    resolve();
                });
            });

            let allUsers = await userDao.search();

            expect(allUsers.length).to.equal(0, 'User was not deleted!');
        });

    });
});