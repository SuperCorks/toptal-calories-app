const httpClient = require('request');
const {expect, fail, assert} = require('test-utils/functions');
const {url} = require('test-utils/request-util');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const server = require('../../bin/server');

const MongoConfigs = require('../../mongo-configs');
const User = require('calories/users/entities/user');
const UserDao = require('calories/users/daos/user-dao');
const {UserModel} = require('calories/users/storage/mongoose-user');
const UserSettings = require('calories/users/entities/user-settings');

const CreateUserEndpoint = "/users";
const TestDatabaseConfigs = {host: 'localhost', database: 'calories-test'};

const userDao = new UserDao();

describe('calories/users/rest-api/user-endpoints POST', () => {

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

    context('POST /users', () => {

        // After tests of "POST /users", remove all users from test database.
        after(() => UserModel.deleteMany());

        it('should create a new user when user has no id', async () => {

            await new Promise((resolve, reject) => {

                let user = new User({
                    username: "test-users@example.com",
                    role: 'member',
                    settings: {dailyCalories: 100}
                });

                httpClient.post(url(CreateUserEndpoint, {body: user}), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        let userId = body.id;
                        expect(userId).to.be.a('string', `Returned user.id isn't a string.`);

                        let returnedUser = new User(body);
                        returnedUser.id = undefined; // clear id for comparison

                        expect(returnedUser).to.eql(user, `Returned user isn't the same as sent user.`);

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });

        it('should update the user when user has an id', async () => {

            // Clear database
            await UserModel.deleteMany();

            let originalUser = await userDao.commit(new User({
                username: "test-users@example.com",
                role: 'member',
                settings: {dailyCalories: 100}
            }));

            await new Promise((resolve, reject) => {

                // Copy user
                let updatedUser = Object.assign({}, originalUser);

                // Make a few modifications
                updatedUser.role = 'manager';
                updatedUser.settings = {dailyCalories: 1200};

                httpClient.post(url(CreateUserEndpoint, {body: updatedUser}), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        let returnedUser = new User(body);

                        expect(returnedUser.id).to.equal(updatedUser.id,
                            `Returned user id isn't the same as sent user id!`
                        );

                        expect(returnedUser).to.eql(updatedUser, `Returned user isn't the same as sent user.`);
                        expect(returnedUser).to.not.eql(originalUser,
                            `Returned user isn't different than the original user even after being modified.`);

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });
        });

    });

    context('POST /users/settings', () => {

        // After tests of "POST /users", remove all users from test database.
        after(() => UserModel.deleteMany());

        it('should update the settings of the user', async () => {

            // Clear database
            await UserModel.deleteMany();

            let theUser = await userDao.commit(new User({
                username: "test-users@example.com",
                role: 'member',
                settings: {dailyCalories: 100}
            }));

            let newUserSettings = {dailyCalories: 900};

            await new Promise((resolve, reject) => {

                httpClient.post(url(`/users/settings/${theUser.id}`, {body: newUserSettings}), (error, response, body) => {

                    try {
                        if (error) return reject(error);

                        if (response.statusCode !== 200)
                            return reject(new Error(`Wrong response code '${response.statusCode}'!`));

                        let returnedSettings = new UserSettings(body);

                        expect(returnedSettings).to.eql(newUserSettings, `Returned settings aren't the same as sent settings.`);

                        resolve();
                    } catch (e) {
                        reject(e);
                    }
                });
            });

            let userFromDb = await userDao.get(theUser.username);

            expect(userFromDb.settings).to.eql(newUserSettings, `Settings from database don't match new settings!`);
        });

    });
});